using UnityEngine;
using Unity.Netcode;
public class CharacterMovement : NetworkBehaviour
{
    CharacterController charCntrl;
    [Tooltip("The speed at which the character will move.")]
    public float speed = 5f;
    [Tooltip("The camera representing where the character is looking.")]
    public GameObject cameraObj;
    [Tooltip("Should be checked if using the Bluetooth Controller to move. If using keyboard, leave this unchecked.")]
    public bool joyStickMode;
    public GazeInteractor gazeInteractor;
    public bool adult;

    // Start is called before the first frame update
    void Start()
    {
        charCntrl = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner) {
            //Get horizontal and Vertical movements
            float horComp = Input.GetAxis("Horizontal");
            float vertComp = Input.GetAxis("Vertical");

            if (joyStickMode)
            {
                horComp = Input.GetAxis("Horizontal");
                vertComp = Input.GetAxis("Vertical");
            }

            Vector3 moveVect = Vector3.zero;

            //Get look Direction
            Vector3 cameraLook = cameraObj.transform.forward;
            cameraLook.y = 0f;
            cameraLook = cameraLook.normalized;

            Vector3 forwardVect = cameraLook;
            Vector3 rightVect = Vector3.Cross(forwardVect, Vector3.up).normalized * -1;

            moveVect += rightVect * horComp;
            moveVect += forwardVect * vertComp;

            moveVect *= speed;
    
            charCntrl.SimpleMove(moveVect);

        }
    }

    public void SetRole(bool adult = false) {
        if (adult) {
            speed = 5f;
            charCntrl.height = 4;
            gazeInteractor.raycastLength = 10;
            charCntrl.enabled = false;
            transform.position = new Vector3(transform.position.x, 2.5f, transform.position.z);
            charCntrl.enabled = true;
        } else {
            speed = 8f;
            charCntrl.height = 1;
            gazeInteractor.raycastLength = 5;
        }
    }

    public void ToggleRole() {
        adult = !adult;
        if (adult) {
            speed = 5f;
            charCntrl.height = 3;
            gazeInteractor.raycastLength = 10;
            charCntrl.enabled = false;
            transform.position = new Vector3(transform.position.x, 2.5f, transform.position.z);
            charCntrl.enabled = true;
        } else {
            speed = 8f;
            charCntrl.height = 1;
            gazeInteractor.raycastLength = 5;
        }
    }
}
