using UnityEngine;
using System.Collections;

public class MoveRacket : MonoBehaviour {
    public float speed = 30;
    public float Movement { get; internal set; }
    private Rigidbody2D _rigibody;

    private void Awake()
    {
        _rigibody = GetComponent<Rigidbody2D>();
    }

    public void UpdateLogic() 
    {
        _rigibody.velocity = new Vector2(0, Movement) * speed;
    }
}
