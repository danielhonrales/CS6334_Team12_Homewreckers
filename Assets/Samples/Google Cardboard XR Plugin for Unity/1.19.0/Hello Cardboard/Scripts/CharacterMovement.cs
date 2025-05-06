using UnityEngine;
using Unity.Netcode;
using System.Collections;
public class CharacterMovement : NetworkBehaviour
{
    CharacterController charCntrl;
    Animator animator;
    [Tooltip("The speed at which the character will move.")]
    public float speed = 5f;
    [Tooltip("The camera representing where the character is looking.")]
    public GameObject cameraObj;
    [Tooltip("Should be checked if using the Bluetooth Controller to move. If using keyboard, leave this unchecked.")]
    public bool joyStickMode;
    public GazeInteractor gazeInteractor;
    public GameObject avatar;

    public Vector3 prevPos;
    public NetworkVariable<bool> adult = new(
        false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // Start is called before the first frame update
    void Start()
    {
        charCntrl = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>(); 

        prevPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        if (adult.Value)
        {
            speed = 5f;
            charCntrl.height = 3;
            gazeInteractor.raycastLength = 10;
            avatar.transform.localScale = new Vector3(2f, 2f, 2f);
            avatar.transform.localPosition = new Vector3(0, -1.8f, 0);
        }
        else
        {
            speed = 8f;
            charCntrl.height = 1;
            gazeInteractor.raycastLength = 5;
            avatar.transform.localScale = new Vector3(1f, 1f, 1f);
            avatar.transform.localPosition = new Vector3(0, -0.8f, 0);
        }

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
            float movementSpeed = new Vector2(moveVect.x, moveVect.z).magnitude;
            if (animator != null)
            {
                animator.SetFloat("Speed", movementSpeed);
            }
        } else {
            if (animator != null)
            {
                if ((prevPos - transform.position).magnitude > .001f) {
                    animator.SetFloat("Speed", 1);
                } else {
                    animator.SetFloat("Speed", 0);
                }
                prevPos = transform.position;
            }
        }
    }

    public void ToggleRole() {
        SetRoleServerRpc(!adult.Value);
        if (adult.Value) {
            charCntrl.enabled = false;
            transform.position = new Vector3(transform.position.x, 2.5f, transform.position.z);
            charCntrl.enabled = true;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SetRoleServerRpc(bool value)
    {
        adult.Value = value;
    }
}
