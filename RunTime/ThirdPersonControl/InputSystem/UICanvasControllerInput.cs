using UnityEngine;

namespace StarterAssets
{
    public class UICanvasControllerInput : MonoBehaviour
    {

        [Header("Output")]
        public CharacterInputData characterInputData;

        public void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            characterInputData.MoveInput(virtualMoveDirection);
        }

        public void VirtualLookInput(Vector2 virtualLookDirection)
        {
            characterInputData.LookInput(virtualLookDirection);
        }

        public void VirtualJumpInput(bool virtualJumpState)
        {
            characterInputData.JumpInput(virtualJumpState);
        }

        public void VirtualSprintInput(bool virtualSprintState)
        {
            characterInputData.SprintInput(virtualSprintState);
        }
        
    }

}
