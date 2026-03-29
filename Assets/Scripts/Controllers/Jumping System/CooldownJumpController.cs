using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Jump;

public class JumpCooldownController : MonoBehaviour
{
    [SerializeField] private JumpProvider jumpProvider;
    [SerializeField] private float jumpCooldown = 0.5f;

    // Bypass qui bloque toujours l'input
    private class BlockedInputReader : IXRInputButtonReader
    {
        public bool ReadIsPerformed() => false;
        public bool ReadWasPerformedThisFrame() => false;
        public bool ReadWasCompletedThisFrame() => false;
        public float ReadValue() => 0f;
        public bool TryReadValue(out float value) { value = 0f; return false; }
    }

    private readonly BlockedInputReader m_BlockedReader = new BlockedInputReader();

    private void OnEnable()
    {
        jumpProvider.locomotionStarted += OnJumpStarted;
    }

    private void OnDisable()
    {
        jumpProvider.locomotionStarted -= OnJumpStarted;
    }

    private void OnJumpStarted(LocomotionProvider provider)
    {
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        jumpProvider.jumpInput.bypass = m_BlockedReader;
        yield return new WaitForSeconds(jumpCooldown);
        jumpProvider.jumpInput.bypass = null;
    }
}