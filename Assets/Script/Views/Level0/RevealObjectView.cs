using UnityEngine;
public class RevealObjectView : MonoBehaviour
{
    [SerializeField] private GameObject objectToReveal;

    public void ShowObject()
    {
        objectToReveal.SetActive(true);
    }

    public void HideObject()
    {
        objectToReveal.SetActive(false);
    }
}