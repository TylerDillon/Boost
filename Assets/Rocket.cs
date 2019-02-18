using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {
    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State {Alive, Dying, Transcending};
    State state = State.Alive;

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 50f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip victorySound;
    [SerializeField] AudioClip levelLoad;

    [SerializeField] ParticleSystem thrustParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem victoryParticles;

    [SerializeField] bool CollisionsEnabled = true;



    // Start is called before the first frame update
    void Start(){
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    private void ProcessInput() {
        if (state == State.Alive) {
            RespondToThrustInput();
            RespondToRotateInput();
        }

        if (Debug.isDebugBuild) {
            if (Input.GetKeyDown(KeyCode.L)) {
                LoadNextScene();
            }
            if (Input.GetKeyDown(KeyCode.C)) {
                CollisionsEnabled = !CollisionsEnabled;
            }
        }
        
    }

    private void RespondToRotateInput() {
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

    private void RespondToThrustInput() {
        if (Input.GetKey(KeyCode.Space)) {
            ApplyThrust();
        }
        else {
            audioSource.Stop();
            thrustParticles.Stop();
        }
    }

    private void ApplyThrust() {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust);
        if (!audioSource.isPlaying) { // so it doesn't layer audio
            audioSource.PlayOneShot(mainEngine);
        }
        thrustParticles.Play();
    }

    private void OnCollisionEnter(Collision collision) {
        if (state != State.Alive || !CollisionsEnabled) {
            return;
        }
        
            switch (collision.gameObject.tag) {
                case "Friendly":
                    print("OK");
                    break;
                case "Finish":
                    state = State.Transcending;
                    audioSource.Stop();
                    audioSource.PlayOneShot(victorySound);
                    victoryParticles.Play();
                    Invoke("LoadNextScene", 2f);
                    break;
                default:
                    state = State.Dying;
                    audioSource.Stop();
                    audioSource.PlayOneShot(deathSound);
                    thrustParticles.Stop();
                    deathParticles.Play();
                    Invoke("OnDeath", 3f);
                    break;
            
        }
    }

    private void OnDeath() {
        ReloadScene();
        state = State.Alive;
    }

    private void LoadNextScene() {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextSceneIndex); //TODO allow for more than two levels

        audioSource.PlayOneShot(levelLoad);
        state = State.Alive;
    }

    private void ReloadScene() {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
