
using System;
using System.Collections.Generic;

[Serializable]
public class JsonTutorialData
{
    public List<TutorialData> tutorial_list;
}
[Serializable]
public class TutorialData
{
    public string name;
    public string time;
    public string thumb;
    public List<TutorialContentData> content_list;
}
[Serializable]
public class TutorialContentData
{
    public int content_id;
    public int image_id;
    public string text;
}
