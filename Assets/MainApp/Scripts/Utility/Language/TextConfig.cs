using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TWT.Config
{
    public static class TextConfig
    {
        public const string ERROR_WHILE_SHOOTING_IMAGE = "写真の撮影に失敗しました。\nThetaの設定を確認して\nください。";
        public const string UPLOAD_IMAGE_FAILED = "サーバーアプリとの通信に失敗しました。";
        public const string CAPTURE_IMAGE_FAILED = "写真の撮影に失敗しました。\nこのデバイスの設定を確認して\nください。";
        public const string CAPTURE_IMAGE_SUCCESS = "写真の撮影に失敗しました。\nThetaとの通信状態を確認して\nください。";
        public const string UNKNOWN_ERROR = "原因不明のエラーです。\n通信状態、設定を確認して\nください。";
        public const string CAPTURE_PREVIEW_FAILED = "プレビュー画像を撮影することが失敗します。";
    }
}