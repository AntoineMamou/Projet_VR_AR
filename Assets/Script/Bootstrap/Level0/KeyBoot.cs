using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KeyBoot : MonoBehaviour
{
    [SerializeField] private KeyController keyController;
    [SerializeField] private RevealObjectView revealObjectView;
    [SerializeField] private RevealObjectView revealObjectView2;
    private void Awake()
    {
        revealObjectView.HideObject(); // état initial : caché
        revealObjectView2.HideObject(); // état initial : caché
    }
}
