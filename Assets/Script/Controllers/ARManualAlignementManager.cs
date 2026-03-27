using UnityEngine;
using Unity.XR.CoreUtils;

public class ARManualAlignmentManager : MonoBehaviour
{
    [Header("Références")]
    public XROrigin xrOrigin;
    public GameObject canvasAlignement;

    [Header("Paramètres (Sensibilité)")]
    public float distanceParClic = 0.05f; 
    public float angleParClic = 2.0f;  



    public void ValiderAlignement()
    {
        if (canvasAlignement != null)
        {
            canvasAlignement.SetActive(false);
            Debug.Log("[COLOC MANUELLE] Alignement validé, interface masquée !");
        }
    }

    public void DeplacerMondeAvant()
    {
        Vector3 direction = xrOrigin.Camera.transform.forward;
        direction.y = 0; // On reste plat par rapport au sol
        xrOrigin.transform.position -= direction.normalized * distanceParClic;
    }

    public void DeplacerMondeArriere()
    {
        Vector3 direction = xrOrigin.Camera.transform.forward;
        direction.y = 0;
        xrOrigin.transform.position += direction.normalized * distanceParClic;
    }

    public void DeplacerMondeGauche()
    {
        Vector3 direction = xrOrigin.Camera.transform.right;
        direction.y = 0;
        xrOrigin.transform.position += direction.normalized * distanceParClic;
    }

    public void DeplacerMondeDroite()
    {
        Vector3 direction = xrOrigin.Camera.transform.right;
        direction.y = 0;
        xrOrigin.transform.position -= direction.normalized * distanceParClic;
    }

    public void DeplacerMondeHaut()
    {
        xrOrigin.transform.position -= Vector3.up * distanceParClic;
    }

    public void DeplacerMondeBas()
    {
        xrOrigin.transform.position += Vector3.up * distanceParClic;
    }


    public void TournerMondeGauche()
    {
        xrOrigin.transform.RotateAround(xrOrigin.Camera.transform.position, Vector3.up, angleParClic);
    }

    public void TournerMondeDroite()
    {
        xrOrigin.transform.RotateAround(xrOrigin.Camera.transform.position, Vector3.up, -angleParClic);
    }
}