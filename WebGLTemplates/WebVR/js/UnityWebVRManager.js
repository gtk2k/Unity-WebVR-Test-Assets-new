var vrHMD, vrSensor;
var win = window, nav = navigator, doc = document, cnv = canvas;
var ua = nav.userAgent.toLowerCase();
var isMobile = (ua.indexOf('android') !== -1 && nav.getVRDevices);
var fullscreenchange = canvas.mozRequestFullScreen ? 'mozfullscreenchange' : 'webkitfullscreenchange';
var mdownTime = 0, mdownMonitorRafId = 0;
var appNameEl = appName;


win.screen.orientation.lock('landscape');
cnv.width = window.innerWidth * window.devicePixelRatio;
cnv.height = window.innerHeight * window.devicePixelRatio;
var windowResize = function () {
  cnv.width = window.innerWidth * window.devicePixelRatio;
  cnv.height = window.innerHeight * window.devicePixelRatio;
};

window.addEventListener('resize', windowResize);

if (isMobile) {
  appNameEl.style.fontSize = (14 * window.devicePixelRatio * 2) + 'px';
  btnVR.style.width = btnVR.style.height = (50 * window.devicePixelRatio * 2) + 'px';
}


setTimeout(function () {
  appNameEl.classList.add('hide');
}, 5000);

appNameEl.addEventListener('transitionend', function () {
  if (appNameEl) {
    appNameEl.parentElement.removeChild(appNameEl);
    appNameEl = null;
  }
});

btnVR.addEventListener('mousedown', function () {
  mdownTime = Date.now();
  monitorMousedownTimer();
});

btnVR.addEventListener('click', function () {
  mdownMonitorRafId && cancelAnimationFrame(mdownMonitorRafId);
  ((Date.now() - mdownTime) <= 100) && vrFullScreen();
});

function monitorMousedownTimer() {
  mdownMonitorRafId = requestAnimationFrame(monitorMousedownTimer);
  var mdowDuration = Date.now() - mdownTime;
  if (mdowDuration >= 1000) {
    cancelAnimationFrame(mdownMonitorRafId);
    resetSensor();
  }
}

function resetSensor() {
  vrSensor && vrSensor.resetSensor();
  var msg = doc.createElement('div');
  msg.id = 'msg';
  msg.style.fontSize = (14 * window.devicePixelRatio * 2) + 'px';
  msg.textContent = 'Sensor reseted';
  msg.addEventListener('transitionend', function () {
    doc.body.removeChild(msg);
  });
  doc.body.appendChild(msg);
  setTimeout(function () {
    msg.classList.add('hide');
  }, 3000);
}

function vrFullScreen() {
  if (!vrHMD) {
    alert('Not ready VRDevice');
    return;
  }
  if (canvas.mozRequestFullScreen) {
    canvas.mozRequestFullScreen({ vrDisplay: vrHMD });
  } else if (canvas.webkitRequestFullscreen) {
    canvas.webkitRequestFullscreen({ vrDisplay: vrHMD });
  }
}

document.addEventListener(fullscreenchange, function (event) {
  if (document.mozFullScreenElement || document.webkitFullscreenElement) {
    SendMessage('WebVRCameraSet', 'changeMode', isMobile ? 'mvr' : 'vr');
  } else {
    SendMessage('WebVRCameraSet', 'changeMode', 'normal');
  }
}, false);

function getVRDevices() {
  if (navigator.getVRDevices) {
    navigator.getVRDevices().then(function (devices) {
      for (var i = 0; i < devices.length; i++) {
        if (devices[i] instanceof HMDVRDevice) {
          vrHMD = devices[i];
          getEyeParameters();
          break;
        }
      }
      for (var i = 0; i < devices.length; i++) {
        if (devices[i] instanceof PositionSensorVRDevice &&
          vrHMD.hardwareUnitId == devices[i].hardwareUnitId) {
          vrSensor = devices[i];
          vrSensor.resetSensor();
          getVRSensorState();
          break;
        }
      }
    });
  }
}

function getEyeParameters() {
  var pL = vrHMD.getEyeParameters('left');
  var pR = vrHMD.getEyeParameters('right');
  var trnL = pL.eyeTranslation, trnR = pR.eyeTranslation;
  var fovL = pL.recommendedFieldOfView, fovR = pR.recommendedFieldOfView;
  SendMessage('WebVRCameraSet', 'eye', [trnL.x, fovL.leftDegrees, fovL.rightDegrees, fovL.downDegrees, fovL.upDegrees,
                                        trnR.x, fovR.leftDegrees, fovR.rightDegrees, fovR.downDegrees, fovR.upDegrees].join(','));
}

function getVRSensorState() {
  requestAnimationFrame(getVRSensorState);
  var state = vrSensor.getState();
  var euler = new THREE.Euler().setFromQuaternion(state.orientation);
  SendMessage('WebVRCameraSet', 'euler_xyz', [euler.x, euler.y, euler.z].join(','));
  if (state.position != null) {
    SendMessage('WebVRCameraSet', 'position_xyz', [state.position.x, state.position.y, state.position.z].join(','));
  }
}