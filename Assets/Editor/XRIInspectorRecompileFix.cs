// À placer dans un dossier Editor/ de ton projet.
// Déselectionne automatiquement les objets problématiques avant une
// recompilation pour éviter le MissingReferenceException de l'Inspector.

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

using Unity.Netcode;

[InitializeOnLoad]
public static class XRIInspectorRecompileFix
{
    static XRIInspectorRecompileFix()
    {
        AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
    }

    private static void OnBeforeAssemblyReload()
    {
        if (Selection.activeGameObject == null) return;

        bool hasXRI     = Selection.activeGameObject
                            .GetComponentInChildren<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>(true) != null;
        bool hasNetcode = Selection.activeGameObject
                            .GetComponentInChildren<NetworkObject>(true) != null;

        if (hasXRI || hasNetcode)
            Selection.activeObject = null;
    }
}
#endif