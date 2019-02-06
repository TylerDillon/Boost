using UnityEngine;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource thrustAudio;
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 50f;

    // Start is called before the first frame update
    void Start(){
        rigidBody = GetComponent<Rigidbody>();
        thrustAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    private void ProcessInput() {
        Thrust();
        Rotate();
    }

    private void Rotate() {
        rigidBody.freezeRotation = true; //take manual control of rotation

        
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A)) {
           
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D)) {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false; //resume physics control
    }

    private void Thrust() {
        if (Input.GetKey(KeyCode.Space)) {
            rigidBody.AddRelativeForce(Vector3.up * mainThrust);
            if (!thrustAudio.isPlaying) { // so it doesn't layer audio
                thrustAudio.Play();
            }
        }
        else {
            thrustAudio.Stop();
        }
    }

    private void OnCollisionEnter(Collision collision) {
        switch (collision.gameObject.tag) {
            case "Friendly":
                print("OK");
                break;
            default:
                print("DEAD");
                break;
        }
    }
}
