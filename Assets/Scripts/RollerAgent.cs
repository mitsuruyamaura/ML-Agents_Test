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
    /// �G�[�W�F���g�̃Q�[���I�u�W�F�N�g�������̏�����
    /// </summary>
    public override void Initialize() {
        Reset();
    }

    /// <summary>
    /// �G�[�W�F���g�̃G�s�\�[�h�J�n���̏�����
    /// </summary>
    public override void OnEpisodeBegin() {

        // Agent �������痎�������Ƃ�
        if (transform.position.y < 0) {
            // Agent �̑��x�ƈʒu�������l�Ƀ��Z�b�g
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
            transform.position = new(0, 0.5f, 0);
        }

        // Target �������_���ɔz�u����
        target.position = new (Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
    }

    /// <summary>
    /// �G�[�W�F���g�ɓn���ώ@�l�̐ݒ�(���v8��)
    /// �G�[�W�F���g�̈ʒu�Ƒ��x�A�^�[�Q�b�g�̈ʒu���擾����(�ʒu��x, y, z�ő��xx, z��������8���ڂ𓾂�)
    /// </summary>
    /// <param name="sensor"></param>
    public override void CollectObservations(VectorSensor sensor) {
        sensor.AddObservation(target.position);
        sensor.AddObservation(transform.position);
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.z);
    }

    /// <summary>
    /// �s�����s���ɌĂ΂��R�[���o�b�N
    /// �|���V�[�ɂ���Č��肳�ꂽ�s���ɉ����āu�s�����s�v���s���A
    /// ���̌��ʂɉ����āu��V�擾�v�Ɓu�G�s�\�[�h�����v���s��
    /// ���肳�ꂽ�s���͈����Ƃ��ēn����Ă���
    /// </summary>
    /// <param name="actions">���肳�ꂽ�s��</param>
    public override void OnActionReceived(ActionBuffers actions) {
        // Agent �ɗ͂�������
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.z = actions.ContinuousActions[1];
        rb.AddForce(controlSignal * forceMultiplier);

        // Agent �� Target �̈ʒu�ɓ��������ꍇ
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < 1.42f) {
            //AddReward(1.0f);
            EndEpisode();
        }

        // Agent �������痎�����ꍇ
        if (transform.position.y < 0) {
            EndEpisode();
        }
    }

    /// <summary>
    /// �L�[���͂��g���ꍇ
    /// </summary>
    /// <param name="actionsOut"></param>
    public override void Heuristic(in ActionBuffers actionsOut) {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}