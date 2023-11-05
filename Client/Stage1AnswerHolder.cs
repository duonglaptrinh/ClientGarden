using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1AnswerHolder : MonoBehaviour
{

    public List<Stage1AnswerHolder> Dependencies { get; set; }

    SelectModelAsAnswer[] selectModelAsAnswers;

    public void OnSelect()
    {
        selectModelAsAnswers = GetComponentsInChildren<SelectModelAsAnswer>();

        if (selectModelAsAnswers != null)
        {
            foreach (var e in selectModelAsAnswers)
                e.HightLightThis();
        }

        if (Dependencies != null)
        {
            foreach (var e in Dependencies)
            {
                var models = e.GetComponentsInChildren<SelectModelAsAnswer>();

                if (models != null)
                {
                    foreach (var m in models)
                    {
                        m.SetTransparent();
                    }
                }
            }
        }
    }

    public void OnUnSelect()
    {
        if (selectModelAsAnswers != null)
        {
            foreach (var e in selectModelAsAnswers)
                e.UnHightLightThis();
        }

        if (Dependencies != null)
        {
            foreach (var e in Dependencies)
            {
                var models = e.GetComponentsInChildren<SelectModelAsAnswer>();

                if (models != null)
                {
                    foreach (var m in models)
                    {
                        m.UnSetTransparent();
                    }
                }
            }
        }
    }

}
