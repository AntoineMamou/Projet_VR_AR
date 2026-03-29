using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ARSpawnManager : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level_Design_0")
        {
            // Au lieu de téléporter tout de suite, on lance un compte à rebours
            StartCoroutine(AttendreEtTeleporter());
        }
    }

    private IEnumerator AttendreEtTeleporter()
    {
        // On laisse AR Foundation s'allumer et placer son (0,0,0) pendant 0.5 seconde
        yield return new WaitForSeconds(0.5f);

        GameObject pointDeSpawn = GameObject.Find("SpawnPoint_AR");

        if (pointDeSpawn != null)
        {
            transform.position = pointDeSpawn.transform.position;
            //transform.rotation = pointDeSpawn.transform.rotation;

            Debug.Log("[AR SPAWN] Téléportation FORCÉE après le réveil de l'AR !");
        }
        else
        {
            Debug.LogWarning("[AR SPAWN] Impossible de trouver le SpawnPoint_AR !");
        }
    }
}