using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaitingQueue.Server.Models;
using WaitingQueue.Server.Services;


namespace WaitingQueue.Server.Controllers;

[ApiController]
[Route("api/queue")]
public class QueueController : ControllerBase
{
    private readonly IQueueService _queueService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<QueueController> _logger;

    public QueueController(
        IQueueService queueService,
        ITokenService tokenService,
        ILogger<QueueController> logger)
    {
        _queueService = queueService;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// 사용자를 대기열에 추가하거나 현재 상태를 반환합니다.
    /// </summary>
    /// <param name="request">사용자 ID, 이메일, 메타데이터를 포함하는 요청</param>
    /// <returns>사용자의 대기열 상태 또는 활성 토큰</returns>
    [HttpPost("join")]
    public async Task<IActionResult> JoinQueue([FromBody] JoinQueueRequest request)
    {
        try
        {
            // 1. 요청 유효성 검사: UserId는 필수입니다.
            if (string.IsNullOrEmpty(request.UserId))
            {
                return BadRequest(new { success = false, error = "userId is required" });
            }

            // 2. 사용자의 현재 상태를 확인합니다.
            var currentStatus = await _queueService.GetQueueStatusAsync(request.UserId);

            // 3. 이미 대기 중인 경우, 현재 상태를 그대로 반환합니다.
            if (currentStatus.Status == "waiting")
            {
                return Ok(new { success = true, data = currentStatus });
            }

            // 4. 이미 활성 상태인 경우, 새 액세스 토큰을 발급하여 반환합니다.
            if (currentStatus.Status == "active")
            {
                var accessToken = _tokenService.GenerateAccessToken(request.UserId);
                return Ok(new
                {
                    success = true,
                    data = new { status = "active", accessToken, canAccess = true }
                });
            }

            // 5. 대기열에 없는 신규 사용자 처리
            // 대기열에 사용자를 추가합니다.
            await _queueService.AddToQueueAsync(request.UserId, new UserData
            {
                Email = request.Email,
                Metadata = request.Metadata
            });

            // 즉시 입장이 가능한지 확인하기 위해 대기열을 바로 처리합니다.
            await _queueService.ProcessQueueAsync();

            // 처리 후 변경된 최신 상태를 다시 조회합니다.
            var updatedStatus = await _queueService.GetQueueStatusAsync(request.UserId);

            // 6. 최종 응답 생성
            object responseData = updatedStatus;
            // 만약 상태가 'active'로 변경되었다면, 응답에 액세스 토큰을 포함시킵니다.
            if (updatedStatus.Status == "active")
            {
                var accessToken = _tokenService.GenerateAccessToken(request.UserId);
                responseData = new
                {
                    status = updatedStatus.Status,
                    canAccess = updatedStatus.CanAccess,
                    position = updatedStatus.Position,
                    accessToken // 토큰 추가
                };
            }

            // 201 Created 상태 코드와 함께 최종 결과를 반환합니다.
            return CreatedAtAction(nameof(GetQueueStatus), new { userId = request.UserId }, new { success = true, data = responseData });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while joining the queue for user {UserId}", request.UserId);
            return StatusCode(500, new { success = false, error = "An internal server error occurred." });
        }
    }

    // `GetQueueStatus` 메서드 (CreatedAtAction에서 참조하기 위해 필요)
    [HttpGet("status/{userId}")]
    public async Task<IActionResult> GetQueueStatus(string userId)
    {
        var status = await _queueService.GetQueueStatusAsync(userId);
        if (status.Status == "not_in_queue")
        {
            return NotFound(new { success = false, error = "User not found in queue" });
        }
        return Ok(new { success = true, data = status });
    }

    /// <summary>
    /// 요청 헤더의 JWT 토큰을 검증하여 접근 권한을 확인합니다.
    /// </summary>
    /// <returns>사용자의 접근 유효성</returns>
    [Authorize] // 이 어트리뷰트는 유효한 JWT 토큰이 헤더에 있어야만 메서드가 실행되도록 합니다.
    [HttpPost("verify")]
    public async Task<IActionResult> VerifyAccess()
    {
        try
        {
            // 1. [Authorize] 어트리뷰트가 토큰을 검증했으므로, 토큰 페이로드(Claims)에서 사용자 ID를 안전하게 가져올 수 있습니다.
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                // 토큰에 사용자 ID 클레임이 없는 경우
                return Unauthorized(new { success = false, error = "Invalid token: missing user identifier." });
            }

            // 2. 데이터베이스에서 사용자의 실제 상태를 조회합니다.
            var status = await _queueService.GetQueueStatusAsync(userId);

            // 3. 사용자가 'active' 상태인지 확인하여 접근 유효성을 결정합니다.
            var isValid = status.Status == "active";

            return Ok(new
            {
                success = true,
                data = new
                {
                    valid = isValid,
                    userId,
                    status = status.Status
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during access verification.");
            return StatusCode(500, new { success = false, error = "An internal server error occurred." });
        }
    }

}
