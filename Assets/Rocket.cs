using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
  [SerializeField] float rcsThrust = 200f;
  [SerializeField] float mainThrust = 50f;

  [SerializeField] float levelLoadDelay = 1f;

  [SerializeField] AudioClip mainEngine;
  [SerializeField] AudioClip success;
  [SerializeField] AudioClip death;

  [SerializeField] ParticleSystem mainEngineParticles;
  [SerializeField] ParticleSystem successParticles;
  [SerializeField] ParticleSystem deathParticles;

  Rigidbody rigidbody;
  AudioSource audioSource;

  enum State { Alive, Dying, Transcending }
  State state = State.Alive;
  bool collisionsDisabled = false;

  // Use this for initialization
  void Start()
  {
    rigidbody = GetComponent<Rigidbody>();
    audioSource = GetComponent<AudioSource>();
  }

  // Update is called once per frame
  void Update()
  {
    if (state == State.Alive)
    {
      RespondToThrustInput();
      RespondToRotateInput();
    }

    if (Debug.isDebugBuild)
    {
      RespondToDebugInput();
    }
  }

  void OnCollisionEnter(Collision collision)
  {
    if (state != State.Alive || collisionsDisabled)
    {
      return;
    }

    switch (collision.gameObject.tag)
    {
      case "Friendly":
        // Do nothing
        break;
      case "Finish":
        StartSuccessSequence();
        break;
      default:
        StartDeathSequence();
        break;
    }
  }

  private void StartDeathSequence()
  {
    state = State.Dying;
    audioSource.Stop();
    audioSource.PlayOneShot(death);
    deathParticles.Play();
    Invoke("LoadFirstLevel", levelLoadDelay);
  }

  private void StartSuccessSequence()
  {
    state = State.Transcending;
    audioSource.Stop();
    audioSource.PlayOneShot(success);
    successParticles.Play();
    Invoke("LoadNextLevel", levelLoadDelay);
  }

  // Load Following level upon winning
  private void LoadNextLevel()
  {
    // TODO: allow more than 2 levels
    int currentIndex = SceneManager.GetActiveScene().buildIndex;
    int nextSceneIndex = currentIndex + 1;
    if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
    {
      nextSceneIndex = 0;
    }
    SceneManager.LoadScene(nextSceneIndex);
  }

  private void LoadFirstLevel()
  {
    SceneManager.LoadScene(0);
  }

  // Handle rocket ship thrusting
  private void RespondToThrustInput()
  {
    if (Input.GetKey(KeyCode.Space))
    {
      ApplyThrust();
    }
    else
    {
      StopApplyingThrust();
    }
  }

  private void StopApplyingThrust()
  {
    audioSource.Stop();
    mainEngineParticles.Stop();
  }

  private void ApplyThrust()
  {
    rigidbody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
    if (!audioSource.isPlaying)
    {
      audioSource.PlayOneShot(mainEngine);
      mainEngineParticles.Play();
    }
  }

  // Handle game rocket ship rotation
  private void RespondToRotateInput()
  {
    float rotationSpeed = rcsThrust * Time.deltaTime;

    if (Input.GetKey(KeyCode.A))
    {
      RotateManually(rotationSpeed);
    }
    else if (Input.GetKey(KeyCode.D))
    {
      RotateManually(-rotationSpeed);
    }
  }

  private void RotateManually(float rotationSpeed)
  {
    rigidbody.freezeRotation = true; // freeze physics
    transform.Rotate(Vector3.forward * rotationSpeed);
    rigidbody.freezeRotation = false; // resume physics
  }

  private void RespondToDebugInput()
  {
    if (Input.GetKeyDown(KeyCode.L))
    {
      LoadNextLevel();
    }
    else if (Input.GetKeyDown(KeyCode.C))
    {
      collisionsDisabled = !collisionsDisabled;
    }
  }
}
