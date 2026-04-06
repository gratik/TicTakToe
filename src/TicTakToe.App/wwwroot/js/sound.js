window.tttSound = {
  play: function (name) {
    try {
      if (localStorage.getItem('ttt_muted') === 'true') return;
      let audio = document.getElementById('ttt-audio-' + name);
      if (audio) {
        audio.currentTime = 0;
        audio.play();
      }
    } catch {}
  }
};
