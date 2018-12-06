using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace nkjzm.Mirror
{
    [ExecuteInEditMode]
    public class Mirror : MonoBehaviour
    {
        [SerializeField]
        Camera TargetCamera = null;
        [SerializeField]
        bool EnabledTargetCamera = false;
        [SerializeField]
        Camera ReflectionCamera = null;
        [SerializeField]
        Transform Specular = null;
        [SerializeField]
        Transform Frame = null;
        [SerializeField]
        float Size = 1f;

        void OnEnable()
        {
#if UNITY_EDITOR
            EditorApplication.update += UpdateMirror;
#endif
        }

        void OnDisable()
        {
#if UNITY_EDITOR
            EditorApplication.update -= UpdateMirror;
#endif
        }

        Camera TrackingCamera
        {
            get
            {
                if (!EnabledTargetCamera)
                {
#if UNITY_EDITOR
                    return SceneView.lastActiveSceneView ? SceneView.lastActiveSceneView.camera : null;
#endif
                }
                return TargetCamera;
            }
        }

        void Start()
        {
            if (TargetCamera == null)
            {
                TargetCamera = Camera.main ?? FindObjectOfType<Camera>();
            }
        }

        void Reset()
        {
            ReflectionCamera = GetComponentInChildren<Camera>();
        }

        void Update()
        {
#if !UNITY_EDITOR
            UpdateMirror();
#endif
        }

        void UpdateMirror()
        {
            if (TrackingCamera == null) { return; }

            // カメラから鏡面へのベクトル
            var diff = transform.position - TrackingCamera.transform.position;
            // 鏡面の垂直ベクトル
            var normal = transform.forward;
            // 鏡面からの反射ベクトル
            var reflection = diff + 2 * (Vector3.Dot(-diff, normal)) * normal;
            // 鏡面座標に反転させた反射ベクトルを加算する
            ReflectionCamera.transform.position = transform.position - reflection;
            // 鏡面の方向に向ける
            ReflectionCamera.transform.LookAt(transform.position);
            // カメラ設定の更新
            var distance = Vector3.Distance(transform.position, ReflectionCamera.transform.position);
            ReflectionCamera.nearClipPlane = distance * 0.9f;

            // 鏡面をカメラ方向に向ける
            Specular.rotation = Quaternion.LookRotation(Specular.position - TrackingCamera.transform.position);

            // フレームのサイズを更新
            Frame.localScale = new Vector3(Size, Size, 1);
            // 鏡面のサイズを調整
            var angle = Vector3.Angle(-transform.forward, ReflectionCamera.transform.forward);
            var specularSize = Size + Mathf.Sin(angle * Mathf.Deg2Rad) * 2;
            Specular.localScale = new Vector3(-specularSize, specularSize, 1);

            // 焦点距離と表示したい鏡面サイズから画角(FOV)を計算する
            ReflectionCamera.fieldOfView = 2 * Mathf.Atan(specularSize / (2 * distance)) * Mathf.Rad2Deg;

#if UNITY_EDITOR
            // シーンビュー更新
            SceneView.RepaintAll();
#endif
        }
    }
}