using UnityEngine;

public class KeyBoot : MonoBehaviour
{
    [SerializeField] private KeyController keyController;
    [SerializeField] private RevealObjectView revealObjectView;
    [SerializeField] private RevealObjectView revealObjectView2;
    [SerializeField] private RevealObjectView exitDoorRevealView; // ExitDoor

    private void Awake()
    {
        revealObjectView.HideObject();
        revealObjectView2.HideObject();
        exitDoorRevealView.HideObject(); // état initial : caché
    }
}