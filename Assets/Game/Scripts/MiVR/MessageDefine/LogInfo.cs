public struct LogInfo
{
    public string header;
    public string content;

    public LogInfo(string header, string content)
    {
        this.header = header;
        this.content = content;

        this.header = TranslateCommandLogText(header);
    }

    public string TranslateCommandLogText(string command)
    {
        switch (command)
        {
            case "DISPLAY_TEXT_SC":
                return "文字表示";

            case "DISPLAY_IMG_SC":
                return "画像表示";

            case "CLEAR_IMG_SC":
                return "画像消去";

            case "PLAY_WAV_SC":
                return "音声再生";

            case "STOP_WAV_SC":
                return "音声停止";

            case "OPEN_QUESTION_SC":
                return "4択問題表示";

            case "QUESTION_IMAGE_CHOICES_SC":
                return "画像問題表示";

            case "LOAD_MODEL_SC":
                return "オブジェクト読込";

            case "POSITION_MOVE_SC":
                return "オブジェクト移動";

            case "POSITION_ROTATE_SC":
                return "オブジェクト回転";

            case "PLAY_VIDEO_SC":
                return "映像再生";

            case "SET_WAIT_SC":
                return "指定時間待機";

            case "PLAYER_MOVE_SC":
                return "プレイヤー移動";

            default:
                return command;
        }
    }
}

