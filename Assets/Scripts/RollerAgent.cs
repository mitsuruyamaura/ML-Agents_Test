using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RollerAgent : Agent
{
    public Transform target;
    public float forceMultiplier = 10;
    private Rigidbody rb;


    private void Reset() {
        TryGetComponent(out rb);
    }

    /// <summary>
    /// エージェントのゲームオブジェクト生成時の初期化
    /// </summary>
    public override void Initialize() {
        Reset();
    }

    /// <summary>
    /// エージェントのエピソード開始時の初期化
    /// </summary>
    public override void OnEpisodeBegin() {

        // Agent が床から落下したとき
        if (transform.position.y < 0) {
            // Agent の速度と位置を初期値にリセット
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
            transform.position = new(0, 0.5f, 0);
        }

        // Target をランダムに配置する
        target.position = new (Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
    }

    /// <summary>
    /// エージェントに渡す観察値の設定(合計8個)
    /// エージェントの位置と速度、ターゲットの位置を取得する(位置がx, y, zで速度x, z軸方向の8項目を得る)
    /// </summary>
    /// <param name="sensor"></param>
    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(target.position);
        sensor.AddObservation(transform.position);
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.z);
    }

    /// <summary>
    /// 行動実行時に呼ばれるコールバック
    /// ポリシーによって決定された行動に応じて「行動実行」を行い、
    /// その結果に応じて「報酬取得」と「エピソード完了」を行う
    /// 決定された行動は引数として渡されてくる
    /// </summary>
    /// <param name="actions">決定された行動</param>
    public override void OnActionReceived(ActionBuffers actions) {
        // Agent に力を加える
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];
        rb.AddForce(controlSignal * forceMultiplier);

        // Agent が Target の位置に到着した場合
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < 1.42f) {
            //AddReward(1.0f);
            EndEpisode();
        }

        // Agent が床から落ちた場合
        if (transform.position.y < 0) {
            EndEpisode();
        }
    }

    /// <summary>
    /// キー入力を使う場合
    /// </summary>
    /// <param name="actionsOut"></param>
    public override void Heuristic(in ActionBuffers actionsOut) {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}