using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class RLAcademy : MonoBehaviour
{

    public float[] lesson_num                          = {     1f,      2f,      3f,       4f,       5f,       6f};
    public float[] duration                            = {    60f,     60f,    100f,     120f,     120f,     120f};
    public float[] num_potato                          = {     1f,      2f,      5f,       8f,      10f,      15f};
    public float[] num_carrot                          = {     1f,      2f,      5f,       8f,      10f,      15f};
    public float[] reward_collect_a_vegetable          = {   0.1f,   0.05f,   0.02f,  0.0125f,       0f,       0f};
    public float[] reward_put_a_vegetable_in_right_box = {   0.4f,    0.2f,   0.08f,    0.05f,    0.05f,    0.04f};
    public float[] reward_put_a_vegetable_in_wrong_box = { -0.01f,  -0.01f, -0.001f, -0.0005f, -0.0002f, -0.0001f};
    public float[] reward_collect_wrong_vegetable      = { -0.01f,  -0.01f, -0.001f, -0.0005f, -0.0002f, -0.0001f};
    public float[] reward_colide_with_obstacle         = {-0.001f, -0.001f, -0.001f,  -0.001f,  -0.001f,  -0.001f};

}
