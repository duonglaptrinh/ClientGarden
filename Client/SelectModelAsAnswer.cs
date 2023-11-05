using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_WSA || UNITY_WSA_10_0
using HoloToolkit.Unity.InputModule;
#endif

public class SelectModelAsAnswer : MonoBehaviour
#if UNITY_WSA || UNITY_WSA_10_0
    , IFocusable, IInputClickHandler
#endif
{
    // Outline outline;

    Color normalColor;

    Color highlightColor = Color.red;

    float alpha1 = 1f;

    float alpha2 = 1f;

    float alpha3 = .4f;

    List<Color> cachedColors = new List<Color>();

    Material[] materials;

    MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer != null)
        {
            var mats = meshRenderer.materials;

            if (mats != null)
            {
                foreach (var e in mats)
                {
                    if (e != null)
                    {
                        cachedColors.Add(e.color);

                        var c = e.color;
                        c.a = alpha1;

                        normalColor = c;
                        e.color = normalColor;
                    }
                }
            }
        }
    }

    [ContextMenu("Select it !")]
    public void Select()
    {
        Debug.Log("Select it !");

        var stage1AnswerHolder = GetComponentInParent<Stage1AnswerHolder>();

        if (stage1AnswerHolder != null)
        {
            var stage1Question = GameObject.FindObjectOfType<Stage1Question>();

            stage1Question.SendAnswer(stage1AnswerHolder.gameObject.name);
        }
    }


#if UNITY_WSA || UNITY_WSA_10_0
    public void OnInputClicked(InputClickedEventData eventData)
    {
        Select();
    }

    public void OnFocusEnter()
    {
        var stage1AnswerHolder = GetComponentInParent<Stage1AnswerHolder>();

        if (stage1AnswerHolder != null)
        {
            stage1AnswerHolder.OnSelect();
        }
    }

    public void OnFocusExit()
    {
        var stage1AnswerHolder = GetComponentInParent<Stage1AnswerHolder>();

        if (stage1AnswerHolder != null)
        {
            stage1AnswerHolder.OnUnSelect();
        }
    }

#endif

#if UNITY_EDITOR || UNITY_ANDROID
    public void OnMouseEnter()
    {
        var stage1AnswerHolder = GetComponentInParent<Stage1AnswerHolder>();

        if (stage1AnswerHolder != null)
        {
            stage1AnswerHolder.OnSelect();
        }
    }

    public void OnMouseExit()
    {
        var stage1AnswerHolder = GetComponentInParent<Stage1AnswerHolder>();

        if (stage1AnswerHolder != null)
        {
            stage1AnswerHolder.OnUnSelect();
        }
    }

    public void OnMouseDown()
    {
        Select();
    }
#endif

    public void HightLightThis()
    {
        if (meshRenderer != null)
        {
            var mats = meshRenderer.materials;

            if (mats != null)
            {
                foreach (var e in mats)
                {
                    if (e != null)
                    {
                        // var c = e.color;
                        // c.a = alpha2;
                        e.color = highlightColor;
                    }
                }
            }
        }
    }

    public void UnHightLightThis()
    {
        if (meshRenderer != null)
        {
            var mats = meshRenderer.materials;
            try
            {
                if (mats != null)
                {
                    for (int i = 0; i < mats.Length; i++)
                    {
                        var imat = mats[i];
                        var color = cachedColors[i];

                        imat.color = color;
                    }
                }
            }
            catch (System.Exception)
            {

            }

        }
    }


    public void SetTransparent()
    {
        if (meshRenderer != null)
        {
            var mats = meshRenderer.materials;

            if (mats != null)
            {
                foreach (var e in mats)
                {
                    if (e != null)
                    {
                        var c = normalColor;
                        c.a = alpha3;
                        e.color = c;
                    }
                }
            }
        }
    }

    public void UnSetTransparent()
    {

        if (meshRenderer != null)
        {
            var mats = meshRenderer.materials;

            if (mats != null)
            {
                foreach (var e in mats)
                {
                    if (e != null)
                    {
                        var c = normalColor;
                        c.a = alpha2;
                        e.color = c;
                    }
                }
            }
        }
    }

}
