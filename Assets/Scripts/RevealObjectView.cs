using UnityEngine;

[DisallowMultipleComponent]
public class RevealObjectView : MonoBehaviour
{
    [SerializeField] private GameObject objectToReveal;

    public void ShowObject()
    {
        if (objectToReveal == null)
        {
            Debug.LogWarning("[RevealObjectView] No object assigned to reveal.");
            return;
        }

        objectToReveal.SetActive(true);
    }

    public void HideObject()
    {
        if (objectToReveal == null)
        {
            Debug.LogWarning("[RevealObjectView] No object assigned to hide.");
            return;
        }

        objectToReveal.SetActive(false);
    }
}
