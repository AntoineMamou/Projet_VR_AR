using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KeyBoot : MonoBehaviour
{
    [SerializeField] private KeyController keyController;
    [SerializeField] private RevealObjectView revealObjectView;
    private void Awake()
    {
        revealObjectView.HideObject(); // état initial : caché
    }
}
