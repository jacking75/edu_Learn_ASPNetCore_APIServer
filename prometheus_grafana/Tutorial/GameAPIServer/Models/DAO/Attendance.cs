﻿using System;

namespace GameAPIServer.Models.DAO;

public class GdbAttendanceInfo
{
    public int uid { get; set; }
    public int attendance_cnt { get; set; }
    public DateTime recent_attendance_dt { get; set; }
}
