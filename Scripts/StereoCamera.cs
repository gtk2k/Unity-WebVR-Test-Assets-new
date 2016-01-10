﻿using UnityEngine;
using System.Collections;
using System;

public class StereoCamera : MonoBehaviour
{

  Camera cameraMain, cameraL, cameraR;
  float eyeLFOVUpTan, eyeLFOVDownTan, eyeLFOVLeftTan, eyeLFOVRightTan;
  float eyeRFOVUpTan, eyeRFOVDownTan, eyeRFOVLeftTan, eyeRFOVRightTan;
  const float DEG2RAD = (float)(Math.PI / 180);
  Transform myTransform, cameraLTransform, cameraRTransform;
  Vector3 webVREuler = new Vector3();
  Vector3 webVRPosition = new Vector3();
  bool isMobile = true;

  // Use this for initialization
  void Start()
  {
    cameraMain = GameObject.Find("CameraMain").GetComponent<Camera>();
    cameraL = GameObject.Find("CameraL").GetComponent<Camera>();
    cameraR = GameObject.Find("CameraR").GetComponent<Camera>();

    myTransform = this.transform;
    cameraLTransform = cameraL.transform;
    cameraRTransform = cameraR.transform;

    changeMode("normal");

    Application.ExternalCall("getVRDevices");
  }

  // Update is called once per frame
  void Update()
  {
    var unityEuler = ConvertWebVREulerToUnity(webVREuler);
    unityEuler.x = -unityEuler.x;
    unityEuler.z = -unityEuler.z;
    myTransform.rotation = Quaternion.Euler(unityEuler);
    if (!isMobile)
    {
      var pos = webVRPosition * 10;
      pos.z *= -1;
      myTransform.position = pos;
    }
  }

  #region receive values form JavaScript old

  void eye(string arg)
  {
    string[] vals = arg.Split(',');
    cameraLTransform.position.Set(float.Parse(vals[0]), 0, 0);
    cameraL.projectionMatrix = PerspectiveOffCenter(
      -(float)Math.Tan(float.Parse(vals[1]) * DEG2RAD) * cameraMain.nearClipPlane,
      (float)Math.Tan(float.Parse(vals[2]) * DEG2RAD) * cameraMain.nearClipPlane,
      -(float)Math.Tan(float.Parse(vals[3]) * DEG2RAD) * cameraMain.nearClipPlane,
      (float)Math.Tan(float.Parse(vals[4]) * DEG2RAD) * cameraMain.nearClipPlane,
      cameraMain.nearClipPlane, cameraMain.farClipPlane);

    cameraRTransform.position.Set(float.Parse(vals[5]), 0, 0);
    cameraR.projectionMatrix = PerspectiveOffCenter(
      -(float)Math.Tan(float.Parse(vals[6]) * DEG2RAD) * cameraMain.nearClipPlane,
        (float)Math.Tan(float.Parse(vals[7]) * DEG2RAD) * cameraMain.nearClipPlane,
      -(float)Math.Tan(float.Parse(vals[8]) * DEG2RAD) * cameraMain.nearClipPlane,
        (float)Math.Tan(float.Parse(vals[9]) * DEG2RAD) * cameraMain.nearClipPlane,
      cameraMain.nearClipPlane, cameraMain.farClipPlane);
  }

  void euler_xyz(string arg)
  {
    string[] vals = arg.Split(',');
    webVREuler.x = float.Parse(vals[0]);
    webVREuler.y = float.Parse(vals[1]);
    webVREuler.z = float.Parse(vals[2]);
  }

  void position_xyz(string arg)
  {
    string[] vals = arg.Split(',');
    webVRPosition.x = float.Parse(vals[0]);
    webVRPosition.y = float.Parse(vals[1]);
    webVRPosition.z = float.Parse(vals[2]);
  }

  #endregion

  #region receive values form JavaScript old

  void changeMode(string mode)
  {
    switch (mode)
    {
      case "normal":
        cameraMain.enabled = true;
        cameraL.enabled = false;
        cameraR.enabled = false;
        break;
      case "vr":
      case "mvr":
        isMobile = (mode == "mvr");
        cameraMain.enabled = false;
        cameraL.enabled = true;
        cameraR.enabled = true;
        break;
    }
  }

  #endregion

  static Matrix4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
  {
    float x = 2.0F * near / (right - left);
    float y = 2.0F * near / (top - bottom);
    float a = (right + left) / (right - left);
    float b = (top + bottom) / (top - bottom);
    float c = -(far + near) / (far - near);
    float d = -(2.0F * far * near) / (far - near);
    float e = -1.0F;
    Matrix4x4 m = new Matrix4x4();
    m[0, 0] = x;
    m[0, 1] = 0;
    m[0, 2] = a;
    m[0, 3] = 0;
    m[1, 0] = 0;
    m[1, 1] = y;
    m[1, 2] = b;
    m[1, 3] = 0;
    m[2, 0] = 0;
    m[2, 1] = 0;
    m[2, 2] = c;
    m[2, 3] = d;
    m[3, 0] = 0;
    m[3, 1] = 0;
    m[3, 2] = e;
    m[3, 3] = 0;
    return m;
  }



  /// <summary>
  /// Converts the given XYZ euler rotation taken from Threejs to a Unity Euler rotation
  /// </summary>
  Vector3 ConvertWebVREulerToUnity(Vector3 eulerThreejs)
  {
    eulerThreejs.x *= -1;
    eulerThreejs.z *= -1;
    Matrix4x4 threejsMatrix = CreateRotationalMatrixThreejs(ref eulerThreejs);

    Matrix4x4 unityMatrix = threejsMatrix;
    unityMatrix.m02 *= -1;
    unityMatrix.m12 *= -1;
    unityMatrix.m20 *= -1;
    unityMatrix.m21 *= -1;

    Quaternion rotation = ExtractRotationFromMatrix(ref unityMatrix);
    return rotation.eulerAngles;
  }

  /// <summary>
  /// Creates a rotation matrix for the given threejs euler rotation
  /// </summary>
  Matrix4x4 CreateRotationalMatrixThreejs(ref Vector3 eulerThreejs)
  {
    float c1 = Mathf.Cos(eulerThreejs.x);
    float c2 = Mathf.Cos(eulerThreejs.y);
    float c3 = Mathf.Cos(eulerThreejs.z);
    float s1 = Mathf.Sin(eulerThreejs.x);
    float s2 = Mathf.Sin(eulerThreejs.y);
    float s3 = Mathf.Sin(eulerThreejs.z);
    Matrix4x4 threejsMatrix = new Matrix4x4();
    threejsMatrix.m00 = c2 * c3;
    threejsMatrix.m01 = -c2 * s3;
    threejsMatrix.m02 = s2;
    threejsMatrix.m10 = c1 * s3 + c3 * s1 * s2;
    threejsMatrix.m11 = c1 * c3 - s1 * s2 * s3;
    threejsMatrix.m12 = -c2 * s1;
    threejsMatrix.m20 = s1 * s3 - c1 * c3 * s2;
    threejsMatrix.m21 = c3 * s1 + c1 * s2 * s3;
    threejsMatrix.m22 = c1 * c2;
    threejsMatrix.m33 = 1;
    return threejsMatrix;
  }

  /// <summary>
  /// Extract rotation quaternion from transform matrix.
  /// </summary>
  /// <param name="matrix">Transform matrix. This parameter is passed by reference
  /// to improve performance; no changes will be made to it.</param>
  /// <returns>
  /// Quaternion representation of rotation transform.
  /// </returns>
  Quaternion ExtractRotationFromMatrix(ref Matrix4x4 matrix)
  {
    Vector3 forward;
    forward.x = matrix.m02;
    forward.y = matrix.m12;
    forward.z = matrix.m22;

    Vector3 upwards;
    upwards.x = matrix.m01;
    upwards.y = matrix.m11;
    upwards.z = matrix.m21;

    return Quaternion.LookRotation(forward, upwards);
  }

}