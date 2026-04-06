window.tttSound = (function () {
    let _ctx = null;

    function getCtx() {
        if (!_ctx) _ctx = new (window.AudioContext || window.webkitAudioContext)();
        return _ctx;
    }

    function beep(frequency, duration, type, gainValue) {
        try {
            const ctx = getCtx();
            const oscillator = ctx.createOscillator();
            const gain = ctx.createGain();
            oscillator.connect(gain);
            gain.connect(ctx.destination);
            oscillator.type = type || 'sine';
            oscillator.frequency.setValueAtTime(frequency, ctx.currentTime);
            gain.gain.setValueAtTime(gainValue || 0.25, ctx.currentTime);
            gain.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + duration);
            oscillator.start(ctx.currentTime);
            oscillator.stop(ctx.currentTime + duration);
        } catch (e) {}
    }

    return {
        play: function (name) {
            try {
                if (localStorage.getItem('ttt_muted') === 'true') return;
                if (name === 'move') {
                    beep(440, 0.08, 'sine', 0.15);
                } else if (name === 'win') {
                    beep(523, 0.12, 'sine', 0.25);
                    setTimeout(() => beep(659, 0.12, 'sine', 0.25), 130);
                    setTimeout(() => beep(784, 0.2,  'sine', 0.25), 260);
                } else if (name === 'draw') {
                    beep(330, 0.15, 'triangle', 0.2);
                    setTimeout(() => beep(277, 0.25, 'triangle', 0.2), 180);
                }
            } catch (e) {}
        }
    };
})();
