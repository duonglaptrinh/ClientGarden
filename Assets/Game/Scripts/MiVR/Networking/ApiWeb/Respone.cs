[System.Serializable]
public class CodeRespone
{
    public bool status;
    public CodeData data;
}

[System.Serializable]
public class CodeData
{
    public bool code_check;
    public int error_code;
    public string message;
    public string mac_address;
    public string licence_code;
    public string end_date;
}
[System.Serializable]
public class CodeErrorRespone
{
    public bool status;
    public CodeErrorData data;
}

[System.Serializable]
public class CodeErrorData
{
    public bool code_check;
    public int error_code;
    public string message;
}