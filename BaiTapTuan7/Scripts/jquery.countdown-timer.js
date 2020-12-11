(function($) {
  // test  _countdown_now=20xx/xx/xx.xx:xx
  const testNowMatch = window.location.href.match(/(?:\?|&)_countdown_now=([^&]+)/);
  const debugAddTime = (testNowMatch) ? (new Date(testNowMatch[1].replace('.', ' '))).getTime() - (new Date()).getTime() : 0;

  const timeUnit = {
    day:  24 * 60 * 60 * 1000,
    hour: 60 * 60 * 1000,
    min:  60 * 1000,
    sec:  1000,
    ms:   1
  };
  const limiterTimeUnit = {
    day:  timeUnit.day / timeUnit.hour,
    hour: timeUnit.hour / timeUnit.min,
    min:  timeUnit.min / timeUnit.sec,
    sec:  timeUnit.sec / timeUnit.ms,
    ms:   null
  };

  let renderId = 0;
  const timeEvent = $('<div>');
  const timeCounterList = {};
  const timeSegmentList = {};

  const getRenderId = function() {
    return ++ renderId;
  };

  const initEl = function($el) {
    let html = $el.html();
    let limiterUnit = '';
    $.each(['day', 'hour', 'min', 'sec', 'ms'], function() {
      const unit = this;
      const re = new RegExp('{' + unit + '(?::(0+))?}');
      if (html.match(re)) {
        html = html.replace(re,
          '<span class="countdown-timer countdown-timer-' + unit + '"'
          + ' data-countdown-timer-unit="' + unit + '"'
          + ' data-countdown-timer-zero="$1"'
          + ' data-countdown-timer-limiter="' + limiterUnit + '"></span>'
        );
        limiterUnit = limiterTimeUnit[unit];
      }
    });
    $el.html(html);
  };

  const renderEl = function($el, countdownTime) {
    $el.find('span.countdown-timer').each(function(index, el) {
      const $el = $(el);
      timeEvent.on(
        'render-' + countdownTime + '-' + $el.data('countdown-timer-unit')
        + '.' + renderId,
        function(e, time) {
          const unit = $el.data('countdown-timer-unit');
          const limiter = $el.data('countdown-timer-limiter');
          const zero = $el.data('countdown-timer-zero') + '';

          if (limiter > 0)
            time = time % limiter;
          if (unit == 'ms') {
              time = ('00' + time).slice(-3);
              if (zero.length)
                time = time.slice(0, zero.length);
          } else {
            if ((time + '').length < zero.length)
              time = (Array(zero.length).join('0') + time).slice(-zero.length);
          }
          $el.text(time);
        }
      );
    });
  };

  const resetTimeSegment = function(countdownTime) {
    timeSegmentList[countdownTime] = {
      day: null,
      hour: null,
      min: null,
      sec: null,
      ms: null
    };
  };

  const timeCounter = function(countdownTime) {
    resetTimeSegment(countdownTime);

    if (countdownTime in timeCounterList) return;
    timeCounterList[countdownTime] = true;

    let start = false;

    const intervalId = setInterval(function () {
      const now = new Date();
      if (debugAddTime) now.setTime(now.getTime() + debugAddTime); // debug

      let diffTime = countdownTime - now;
      if (diffTime < 0) diffTime = 0;

      $.each(['day', 'hour', 'min', 'sec', 'ms'], function() {
        const unit = this;
        const time  = Math.floor(diffTime / timeUnit[unit]);
        if (timeSegmentList[countdownTime][unit] !== time) {
          timeSegmentList[countdownTime][unit] = time;
          timeEvent.trigger('render-' + countdownTime + '-' + unit, time);
        }
      });
      if (diffTime == 0) {
        clearInterval(intervalId);
        delete timeCounterList[countdownTime];
        timeEvent.trigger('end-' + countdownTime);
      }
    }, 10);
  };

  $.fn.countdownTimer = function(end, endFunction) {
    const countdownTime = (new Date(end)).getTime();

    this.each(function(index, el) {
      const $el = $(el);
      const renderId = getRenderId();

      if ($el.data('countdown-timer-render-id')) {
        timeEvent.off('.' + $el.data('countdown-timer-render-id'));
        $el.off('.' + $el.data('countdown-timer-render-id'));
      }
      $el.data('countdown-timer-render-id', renderId);

      initEl($el);
      renderEl($el, countdownTime);

      $.each(['day', 'hour', 'min', 'sec', 'ms'], function() {
        const unit = this;
        timeEvent.on('render-' + countdownTime + '-' + unit + '.' + renderId, function(e, time) {
          $el.trigger('countdownTimer:render-' + unit, time);
        });
      });
      timeEvent.on('end-' + countdownTime + '.' + renderId, function(e) {
        timeEvent.off('.' + renderId);
        $el.trigger('countdownTimer:end');
      });

      if (typeof(endFunction) == 'function') {
        $el.on('countdownTimer:end.' + renderId, endFunction);
      }
    });

    timeCounter(countdownTime);
    return this;
  };
})(jQuery);
