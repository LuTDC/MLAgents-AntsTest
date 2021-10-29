using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AIAgent : Agent
{
    [SerializeField]
    private Transform leaf, target;

    private bool visitedLeaf = false, canChange = true;

    private Vector3 startingPosition;

    void Start(){
        startingPosition = transform.position;
    }

    public override void OnEpisodeBegin()
    {
        transform.position = startingPosition;
        visitedLeaf = false;
        canChange = true;
        SetReward(1f);
    }

    public override void CollectObservations(VectorSensor sensors){
        sensors.AddObservation(transform.position);
        sensors.AddObservation(leaf.position);
        sensors.AddObservation(target.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        float moveSpeed = 10f;
        transform.position += new Vector3(moveX, moveY, 0) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.tag == "Goal"){
            if(visitedLeaf){
                AddReward(2f);
                Debug.Log("Consegui!");
            }
            else AddReward(-1f);

            EndEpisode();
        }
        else if(other.tag == "Leaf" && canChange){
            if(!visitedLeaf){
                visitedLeaf = true;
                AddReward(1f);
            }
            else{
                AddReward(-1f);
                EndEpisode();
            }

            canChange = false;
        }
        else if(other.tag == "Wall"){
            AddReward(-1f);
            EndEpisode();
        }
    }

    private void OnTriggerExit2D(Collider2D other){
        if(other.tag == "Leaf") canChange = true;
    }
}
