(function () {
    document.getElementById('loading-screen').style.display = 'flex';
    document.getElementById('app').style.display = 'none';

    let progress = 0;
    const progressFill = document.getElementById('progress-fill');

    function updateProgress() {
        if (progress < 90) {
            progress += Math.random() * 5;
            if (progress > 90) {
                progress = 90;
            }

            progressFill.style.width = progress + '%';
            setTimeout(updateProgress, 300);
        }
    }

    updateProgress();

    window.hideLoadingScreen = function () {
        progressFill.style.width = '100%';
        document.documentElement.style.setProperty('--blazor-load-percentage', '100%');

        setTimeout(() => {
            document.getElementById('loading-screen').style.display = 'none';
            document.getElementById('app').style.display = 'block';
        }, 100);
    };
})();
