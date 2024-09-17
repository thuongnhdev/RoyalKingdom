using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConfigVariables", menuName = "Uniflow/CreateConfigVariables")]
public class ConfigVariables : ScriptableObject
{
    public float DISTANCE_FILL_TARGET;
    public float PERCENT_FILL_TARGET = 0.1f;
    public float DISTANCE_TO_DRAG_OBJECT = 0.05f;
    public float DISTANCE_FINGER;
    public float TIME_HIDE_OBJ_GUIDLINE;
    public float TIME_SHOW_OBJ_GUIDLINE;
    public float TIME_FILL_OBJECT;
    public float TIME_NEXT_LAYER;
    public float SCALE_OBJECT_TO = 1.2f;
    public float SCALE_OBJECT_BACK = 1.0f;
    public float DURATION_SCALE_OBJECT = 0.2f;
    public float TIME_FADDING_POPUP = 0.5f;
    public float TIME_UPDATE_PROGRESS_BAR = 0.5f;
    public float TIME_COOLDOWN_SHOW_TUTORIAL = 2.0f;
    public float SIZE_DRAG_WITH_TARGET_OBJ = 2.0f;
    public float DELAY_FILL_TARGET = 0.03f;
    public float MIN_LOADING_WAITING_TIME = 2.0f;
    public int MAX_POPULATION = 300;
    public float DEFAULT_SPEED_WORKER = 5f;
    public float SLOW_SPEED_WORKER = 2.5f;
    public float DURATION_PIXEL_TITLE_BUILDING_ROTATION = 0.25f;
    public float DURATION_DOOR_WORKERHOUSE = 3.0f;
    public float DURATION_DOOR_WORKERHOUSE_PORTER = 2.0f;
    public int VASSAL_JOB_CLASS_EXP_DIVIDE = 100;
    public int VASSAL_STAT_EXP_DIVIDE = 50;
    public int VASSAL_STAT_POTENTIAL_DIVIDE = 5;
    public int VASSAL_STAT_LUCKY_PROB = 100;
    public int VASSAL_STAT_LUCKY_EXP_PER_WORKLOAD = 5;
    public int PROSPERITY_COEFFICIENT = 1;
    public int ECONOMIC_FIGURE_CONEFFICIENT = 1;
    public int POTENTIAL_VALUE_OF_VASSAL = 1;
    public int DEFAUL_VALUE_OF_VASSAL_JOB = -1;
}
