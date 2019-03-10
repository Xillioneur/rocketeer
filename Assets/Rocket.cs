using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
  [SerializeField] float rcsThrust = 200f;
  [SerializeField] float mainThrust = 50f;

  Rigidbody rigidbody;
  AudioSource audioSource;

  enum State { Alive, Dying, Transcending }
  State state = State.Alive;


  // Use this for initialization
  void Start()
  {
    rigidbody = GetComponent<Rigidbody>();
    audioSource = GetComponent<AudioSource>();
  }

  // Update is called once per frame
  void Update()
  {
    if (state != State.Dying)
    {
      Thrust();
      Rotate();
    }
  }

  void OnCollisionEnter(Collision collision)
  {
    switch (collision.gameObject.tag)
    {
      case "Friendly":
        // Do nothing
        break;
      case "Finish":
        state = State.Transcending;
        Invoke("LoadNextScene", 1f); // TODO: parameter time
        break;
      default:
        state = State.Dying;
        Invoke("Dead", 1f);
        break;
    }
  }

  private void LoadNextScene()
  {
    // TODO: allow more than 2 levels
    SceneManager.LoadScene(1);
  }

  private void Dead()
  {
    SceneManager.LoadScene(0);
  }

  // Handle rocket ship thrusting
  private void Thrust()
  {
    if (Input.GetKey(KeyCode.Space))
    {
      rigidbody.AddRelativeForce(Vector3.up * mainThrust);
      if (!audioSource.isPlaying)
      {
        audioSource.Play();
      }
    }
    else
    {
      audioSource.Stop();
    }
  }

  // Handle game rocket ship rotation
  private void Rotate()
  {
    rigidbody.freezeRotation = true; // freeze physics

    float rotationSpeed = rcsThrust * Time.deltaTime;

    if (Input.GetKey(KeyCode.A))
    {

      transform.Rotate(Vector3.forward * rotationSpeed);
    }
    else if (Input.GetKey(KeyCode.D))
    {
      transform.Rotate(-Vector3.forward * rotationSpeed);
    }

    rigidbody.freezeRotation = false; // resume physics
  }
}
