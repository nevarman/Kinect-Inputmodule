//#define EXPERIMENTAL
using UnityEngine;
using System.Collections;
using UnityEditor;
using Windows.Kinect;
using UnityEditorInternal;

[CustomEditor(typeof(CharacterObject))]
public class CharacterObjectEditor : Editor
{
    private ReorderableList list;
    private CharacterObject _char;
    private Animator anim;
#if EXPERIMENTAL
    bool showAvatarSetup = false;
#endif

    void OnEnable()
    {
        _char = target as CharacterObject;
        anim = _char.GetComponent<Animator>();
#if EXPERIMENTAL
        list = new ReorderableList(serializedObject,
                                   serializedObject.FindProperty("jointNameType"),
                                   true, true, true, true);
        list.drawElementCallback = OnDrawElement;
        list.drawHeaderCallback = OnDrawHeader;
        //list.onAddDropdownCallback = OnAddDropDown;
        list.drawFooterCallback = OnDrawFooter;
        list.elementHeight += 50f;
        showAvatarSetup = EditorPrefs.GetBool("sas", false);
#endif
    }
#if EXPERIMENTAL
    void OnDisable()
    {
        EditorPrefs.SetBool("sas", showAvatarSetup);
    }

    private void OnDrawFooter(Rect rect)
    {
        //throw new System.NotImplementedException();
    }

    void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        var element = list.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 3;
        EditorGUI.PropertyField(
            new Rect(rect.x, rect.y + 5, rect.width / 2f - 50f, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("_name"), GUIContent.none);
        EditorGUI.PropertyField(
            new Rect(rect.width / 2f, rect.y + 5, rect.width / 2f, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("jointType"), GUIContent.none);
        EditorGUI.PropertyField(
            new Rect(rect.x /*+ rect.width - 50*/, rect.y + EditorGUIUtility.singleLineHeight + 5f, rect.width, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("boneTransform"), new GUIContent("Bone"));
        EditorGUI.PropertyField(
            new Rect(rect.x /*+ rect.width - 50*/, rect.y + EditorGUIUtility.singleLineHeight * 2f + 5f, rect.width, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("globalPosition"), new GUIContent("Global pos"));
        EditorGUI.PropertyField(
            new Rect(rect.x /*+ rect.width - 50*/, rect.y + EditorGUIUtility.singleLineHeight * 3f + 5f, rect.width, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("initialPosition"), new GUIContent("Local pos"));
    }
    void OnDrawHeader(Rect rect)
    {
        EditorGUI.LabelField(rect, "Avatar Setup");
    }
#endif
    public override void OnInspectorGUI()
    {
#if EXPERIMENTAL
        EditorGUILayout.Space();
        serializedObject.Update();
        _char.inverse = EditorGUILayout.Toggle("Inverse", _char.inverse);
        _char.rootHips = (Transform)EditorGUILayout.ObjectField("Root hips", _char.rootHips, typeof(Transform)) as Transform;
        showAvatarSetup = EditorGUILayout.Foldout(showAvatarSetup, "Avatar Setup");
        if (showAvatarSetup)
            list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
#endif
#if !EXPERIMENTAL
        DrawDefaultInspector();
#endif
        if (_char.rootHips)
        {
            if (GUILayout.Button("Fill Bone Data With Search Pattern"))
            {
                FillBoneData();
            }
        }
        if (anim != null && anim.avatar != null)
        {
            GUI.color = Color.green;
            if (GUILayout.Button("Fill Bone Data With Avatar Data"))
            {
                FillBoneDataFromAvatar();
                serializedObject.ApplyModifiedProperties();
            }
            GUI.color = Color.white;
        }
    }

    private void FillBoneDataFromAvatar()
    {
        int arrayCount = 0;
        for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++)
        {
            arrayCount++;
        }
        _char.jointNameType = new JointNameType[arrayCount];
        arrayCount = 0;
        for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++)
        {
            Transform t = GetTransformFromAvatar(jt);
            _char.jointNameType[arrayCount] = new JointNameType(jt, t);
            arrayCount++;
        }
    }

    private Transform GetTransformFromAvatar(JointType jt)
    {
        switch (jt)
        {
            case JointType.SpineBase:
                return anim.GetBoneTransform(HumanBodyBones.Hips);
            case JointType.SpineMid:
                return anim.GetBoneTransform(HumanBodyBones.Spine);
            case JointType.SpineShoulder:
                return anim.GetBoneTransform(HumanBodyBones.Chest);
            case JointType.Neck:
                return anim.GetBoneTransform(HumanBodyBones.Neck);
            case JointType.AnkleLeft:
                if (_char.inverse)
                    return anim.GetBoneTransform(HumanBodyBones.RightFoot);
                else
                    return anim.GetBoneTransform(HumanBodyBones.LeftFoot);
            case JointType.AnkleRight:
                if (_char.inverse)
                    return anim.GetBoneTransform(HumanBodyBones.LeftFoot);
                else
                    return anim.GetBoneTransform(HumanBodyBones.RightFoot);
            case JointType.KneeLeft:
                if (_char.inverse)
                    return anim.GetBoneTransform(HumanBodyBones.RightLowerLeg);
                else
                    return anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
            case JointType.KneeRight:
                if (_char.inverse)
                    return anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
                else
                    return anim.GetBoneTransform(HumanBodyBones.RightLowerLeg);
            case JointType.ShoulderLeft:
                if (_char.inverse)
                    return anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
                else
                    return anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            case JointType.ShoulderRight:
                if (_char.inverse)
                    return anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
                else
                    return anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
            case JointType.ElbowLeft:
                if (_char.inverse)
                    return anim.GetBoneTransform(HumanBodyBones.RightLowerArm);
                else
                    return anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            case JointType.ElbowRight:
                if (_char.inverse)
                    return anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
                else
                    return anim.GetBoneTransform(HumanBodyBones.RightLowerArm);
            case JointType.WristLeft:
                if (_char.inverse)
                    return anim.GetBoneTransform(HumanBodyBones.RightHand);
                else
                    return anim.GetBoneTransform(HumanBodyBones.LeftHand);
            case JointType.WristRight:
                if (_char.inverse)
                    return anim.GetBoneTransform(HumanBodyBones.LeftHand);
                else
                    return anim.GetBoneTransform(HumanBodyBones.RightHand);
            case JointType.HipLeft:
                if (_char.inverse)
                    return anim.GetBoneTransform(HumanBodyBones.RightUpperLeg);
                else
                    return anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
            case JointType.HipRight:
                if (_char.inverse)
                    return anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
                else
                    return anim.GetBoneTransform(HumanBodyBones.RightUpperLeg);
            default:
                return null;//anim.GetBoneTransform(HumanBodyBones.Hips);
        }
    }

    private void FillBoneData()
    {
        int arrayCount = 0;
        for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++)
        {
            arrayCount++;
        }
        _char.jointNameType = new JointNameType[arrayCount];
        arrayCount = 0;
        for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++)
        {
            Transform t = GetTransformFromJoint(jt);
            _char.jointNameType[arrayCount] = new JointNameType(jt, t);
            arrayCount++;
        }
    }

    private Transform GetTransformFromJoint(JointType jt)
    {
        switch (jt)
        {
            case JointType.SpineBase:
                return _char.rootHips;
            case JointType.SpineMid:
                return SearchHierachy("Spine1");
            case JointType.SpineShoulder:
                return SearchHierachy("Spine2");
            case JointType.Neck:
                return SearchHierachy("Neck");
            case JointType.AnkleLeft:
                return SearchHierachy(BodyJointSide.Left, "Ankle", "Foot", _char.inverse);
            case JointType.AnkleRight:
                return SearchHierachy(BodyJointSide.Right, "Ankle", "Foot", _char.inverse);
            case JointType.KneeLeft:
                return SearchHierachy(BodyJointSide.Left, "Knee", "Leg", _char.inverse);
            case JointType.KneeRight:
                return SearchHierachy(BodyJointSide.Right, "Knee", "Leg", _char.inverse);
            case JointType.ShoulderLeft:
                return SearchHierachy(BodyJointSide.Left, "Arm", "Arm", _char.inverse);
            case JointType.ShoulderRight:
                return SearchHierachy(BodyJointSide.Right, "Arm", "Arm", _char.inverse);
            case JointType.ElbowLeft:
                return SearchHierachy(BodyJointSide.Left, "Elbow", "ForeArm", _char.inverse);
            case JointType.ElbowRight:
                return SearchHierachy(BodyJointSide.Right, "Elbow", "ForeArm", _char.inverse);
            case JointType.WristLeft:
                return SearchHierachy(BodyJointSide.Left, "Wrist", "Hand", _char.inverse);
            case JointType.WristRight:
                return SearchHierachy(BodyJointSide.Right, "Wrist", "Hand", _char.inverse);
            case JointType.HipLeft:
                return SearchHierachy(BodyJointSide.Left, "Hip", "UpLeg", _char.inverse);
            case JointType.HipRight:
                return SearchHierachy(BodyJointSide.Right, "Hip", "UpLeg", _char.inverse);
            default:
                return _char.rootHips;
        }
    }

    private Transform SearchHierachy(string pattern)
    {
        Transform[] transforms = _char.GetComponentsInChildren<Transform>();
        foreach (Transform t in transforms)
        {
            // Debug.Log(t.name);
            if (t.name.Contains(pattern))
                return t;
        }
        return _char.rootHips;
    }
    private Transform SearchHierachy(BodyJointSide side, string pattern, string alternative, bool inverse)
    {
        Transform[] transforms = _char.GetComponentsInChildren<Transform>();
        foreach (Transform t in transforms)
        {
            // Debug.Log(t.name);
            string sSide;
            if (!inverse)
                sSide = side == BodyJointSide.Left ? "Left" : "Right";
            else
                sSide = side == BodyJointSide.Left ? "Right" : "Left";
            if (t.name.Contains(sSide) && (t.name.Contains(pattern) || t.name.Contains(alternative)))
                return t;
        }
        return _char.rootHips;
    }
}

public enum BodyJointSide
{
    Left, Right
}
