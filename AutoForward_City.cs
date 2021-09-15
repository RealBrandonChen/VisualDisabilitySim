using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Diagnostics;
using UnityEngine.SceneManagement;
using UnityEditor;

public class AutoForward_City : MonoBehaviour {

	Rigidbody m_Rigidbody;

	float	AutoMoveSpeed = 1f;
	public	static	float	m_distanceTraveled = 0f;
	public	static	bool	AutoMove_on = false;
	public	static	bool	Finished = false;
	private			bool	JoyStick_Returned = true;
	public GameObject exit_Canvas;
	public GameObject ControlerModel;
	public GameObject ArrowPedestrain;
	public GameObject ArrowBox;
	public static bool showForwardButton = false;
	public static bool showBackButton = false;

	public string ProjectSelectScene;
	public AudioClip[] ChineseAudios;

	public	static	string 	filename;
	public static string SavePath;
	Transform 	hmd = null;
	Vector3 	oldPosition;

	public AudioClip[] InstructionAudioClip;
	private AudioSource InstructionAudioSource;

	public GameObject[] ObjectHidedInPractice;
	public GameObject[] ObjectActivatedInPractice;
	public GameObject[] ObjectHidedInTutorial;
	public GameObject[] ObjectActivatedInTutorial;

	public static bool isTutorial;
	public static bool isPractice = false;
	public static bool isRealTest = false;
	public static bool isChangingToPrac = false;
	public static bool isChangingToReal = false;

	public static bool isFirstRoute = true;
	public static bool isRouteTest = false;
	public static int routeCount = 0;

	private int instruction_order = 0;
	private float instructionTimeElapsed;
	private float keyHoldTimeElapsed;
	private bool isKeyHoldTimeSatisfied;
	private bool isBackwardDistanceSatisfied;
	private bool isForwardDistanceSatisfied;
	private float pedestrainCollisionPosX;
	private float lastPlayerPosX;

	private bool controllerButton_Forward_O = true;
	private bool controllerButton_Backward_X = true;
	private bool isInstructionAudioActivated = false;
	private bool isInstructionFinished = false;
	private string onCollisionObjectName;
	private String[] actionsDebugList;


	void Awake () {
		hmd = GameObject.Find("Camera (head)").transform; //CenterEyeAnchor
		oldPosition = transform.position;

		m_Rigidbody = GameObject.Find("XRRig").GetComponent<Rigidbody>();
		SetLanguage();	
	}

	void Start () {
		Finished = false;
		m_distanceTraveled = 0f;

		if (!isChangingToPrac && !isChangingToReal)
		{
			isTutorial = true;
			SetObjectInTutorial();
		}
		else if (isChangingToPrac)
		{
			isTutorial = false;
			instruction_order = 21;
			SetObjectInPrac();
		}
		else if (isChangingToReal)
		{
			isTutorial = false;
			if (SettingLight_City.LightLevel != 1)
			{
				SettingLight_City.LightLevel = 3;
			}
		}

		isPractice = isChangingToPrac;
		isRealTest = isChangingToReal;
		isChangingToPrac = false;

		SetSavingPath();

		InstructionAudioSource = gameObject.GetComponents<AudioSource>()[3];
		actionsDebugList = new String[] {
			"Action 0 Finished: Welcome",
			"Action 1 Finished: Introduction",
			"Action 2 Finished: Forward Test",
			"Action 3 Finished: Backward Test1",
			"Action 4 Finished: Backward Test2",
			"Action 5 Finished: Backward Test3",
			"Action 6 Finished: Pavement Walk1",
			"Action 7 Finished: Pavement Walk2",
			"Action 8 Finished: Obstacle1",
			"Action 9 Finished: Pedestrian1",
			"Action 10 Finished: Obstacle2",
			"Action 11 Finished: Pedestrian2",
			"Action 12 Finished: Pedestrian3",
			"Action 13 Finished: BoxCollider1",
			"Action 14 Finished: BoxCollider2",
			"Action 15 Finished: BoxCollider3",
			"Action 16 Finished: Pavement Walk3",
			"Action 17 Finished: Remind The Car",
			"Action 18 Finished: Road Crossing",
			"Action 19 Finished: SecondLast Check Point",
			"Action 20 Finished: End Check Point",
		};
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.M)) {
			isChangingToPrac = true;
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
		CharacterController controller = GetComponent<CharacterController>();
		Quaternion prevRot = GameObject.Find("TrackingSpace2").transform.rotation;
		transform.rotation = Quaternion.Euler(0f,hmd.rotation.eulerAngles.y,0f);
		GameObject.Find ("TrackingSpace2").transform.rotation = prevRot;

        try
        {
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				exit_Canvas.SetActive(true);
			}

			if (Input.GetKeyDown(KeyCode.F1))
			{
				Finished = true;
			}
			if (isRealTest && Finished)
			{
				if (isRouteTest)
				{
					if (routeCount < 9)
					{
						routeCount++;
						SettingBlockage_City.PosChoice++;
						if (SettingBlockage_City.PosChoice > 5)
							SettingBlockage_City.PosChoice -= 5;

						if (isFirstRoute)
							isFirstRoute = false;

						if (routeCount >= 5)
							SettingLight_City.LightLevel = 1;

						SceneManager.LoadScene(SceneManager.GetActiveScene().name);
					}
					else
					{
						StartCoroutine(SelectScene(ProjectSelectScene, 2.0f));
					}
				}
                else
                {
					if (SettingLight_City.LightLevel == 1)
					{
						StartCoroutine(SelectScene(ProjectSelectScene, 2.0f));
					}
					else
					{
						SettingLight_City.LightLevel = 1;
						SceneManager.LoadScene(SceneManager.GetActiveScene().name);
					}
				}
			}

			if (Input.GetKeyUp(KeyCode.Return))
			{
				AutoMove_on = true;
			}
			if (Input.GetKeyUp(KeyCode.Backspace))
			{
				AutoMove_on = false;
			}
			
			if (!Finished && controllerButton_Forward_O &&
				(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.JoystickButton1) || Mathf.Abs(Input.GetAxis("JoyVertical")) > 0.2f))
			{
				AutoMove_on = true;
				AutoMoveSpeed = 1f;
				if (Mathf.Abs(Input.GetAxis("JoyVertical")) > 0.2f && JoyStick_Returned)
					GetComponents<AudioSource>()[2].Play();
				if (Mathf.Abs(Input.GetAxis("JoyVertical")) > 0.2f)
					JoyStick_Returned = false;
				else
					GetComponents<AudioSource>()[2].Play();
			}
			if (!Finished && Input.GetKeyDown(KeyCode.JoystickButton3) && controllerButton_Forward_O)
			{
				AutoMove_on = true;
				AutoMoveSpeed = 2f;
				GetComponents<AudioSource>()[2].Play();
			}
			if (!Finished && Input.GetKeyDown(KeyCode.JoystickButton0) && controllerButton_Backward_X)
			{
				AutoMove_on = true;
				AutoMoveSpeed = -1f;
				GetComponents<AudioSource>()[2].Play();
			}
			if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyUp(KeyCode.JoystickButton1) || Input.GetKeyUp(KeyCode.JoystickButton3) ||
				Input.GetKeyUp(KeyCode.JoystickButton0) || (!JoyStick_Returned && Mathf.Abs(Input.GetAxis("JoyVertical")) < 0.2f) ||
				(!controllerButton_Backward_X && !controllerButton_Forward_O))
			{
				AutoMove_on = false;
				if (!JoyStick_Returned && Mathf.Abs(Input.GetAxis("JoyVertical")) < 0.2f)
					JoyStick_Returned = true;
				GetComponents<AudioSource>()[2].loop = false;
			}

			// Move automatically forward for certain distance when pressed Keypad 6
			if (AutoMove_on == true)
			{
				//checks to see if the distance traveled is less than 100 * f
				if (m_distanceTraveled < 66f)
				{ 

					if (Input.GetAxis("JoyVertical") < -0.2f)
						AutoMoveSpeed = 0.5f * (1f + (Mathf.Abs(Input.GetAxis("JoyVertical")) - 0.2f) / 0.8f * 3f);
					else if (Input.GetAxis("JoyVertical") > 0.2f)
						AutoMoveSpeed = -0.5f * (1f + (Mathf.Abs(Input.GetAxis("JoyVertical")) - 0.2f) / 0.8f * 1f);
					Vector3 moveDirection = new Vector3((Input.GetAxis("Horizontal")), (-Input.GetAxis("Vertical")), 0);
					moveDirection = transform.TransformDirection(Vector3.forward);
					controller.Move(moveDirection * AutoMoveSpeed * Time.deltaTime);
					GetComponents<AudioSource>()[2].loop = true;
					m_distanceTraveled = Mathf.Abs((oldPosition - transform.position).x);
				}
				else
				{
					Timer0812_City.enableTimer = false;
					if (!File.Exists(SavePath + filename))
						using (StreamWriter sw = File.CreateText(SavePath + filename)) { }    
					if (!Finished)
						using (StreamWriter sw = File.AppendText(SavePath + filename))
						{       
							sw.WriteLine(String.Join(", ", new string[] { "Final Result", System.DateTime.Now.ToString("yyyy/MM/dd"), System.DateTime.Now.ToString("HH:mm:ss") }));
							sw.WriteLine(String.Join(", ", new string[] { "Light Level", "Time Used (s)", "No. of Collision", "Obstacle Setting", "Time Touching Blockage (s)", "Distance Travelled (m)", "Patient Age" }));
							sw.WriteLine(String.Join(", ", new string[] { SettingLight_City.LightLevel.ToString(), Timer0812_City.startTime.ToString("F2"),  HitCollisionValue_City.currentHit.ToString(), SettingBlockage_City.PosChoice.ToString(), 
								HealthCollision_City.totalDamage.ToString("F2"), (Mathf.Abs((transform.position - oldPosition).z) + m_distanceTraveled).ToString("F2"), 
								(Convert.ToInt32(System.DateTime.Now.ToString("yyyy")) - Convert.ToInt32(DropdownYearOption.DOBYear)).ToString(),}));
							sw.WriteLine();
						}
					isInstructionFinished = false;
					AutoMove_on = false;
					m_distanceTraveled = 0f;
					GetComponents<AudioSource>()[2].loop = false;
				}
			}
			ActionsTrigger();
		}
		catch (Exception e)
		{
			UnityEngine.Debug.LogFormat(
				 "Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
		}
	}

	private IEnumerator SelectScene(string sceneName, float loadingTime)
	{
		yield return new WaitForSeconds(loadingTime);
		SettingLight_City.LightLevel = 3;
		SceneManager.LoadScene(sceneName);
	}

	private void SetObjectInTutorial()
	{
		SetObjectInPrac();
		
		foreach (GameObject objectActiveInTutorial in ObjectActivatedInTutorial)
        {
			objectActiveInTutorial.SetActive(true);
        }

		foreach (GameObject objectHideInTutorial in ObjectHidedInTutorial)
        {
			objectHideInTutorial.SetActive(false);
        }

		GameObject.Find("Blockage 0203 - toilet1").transform.position = new Vector3(-49.65f, 0.68f, 10.96f);
		GameObject.Find("CardboardBox_0").transform.position = new Vector3(-54.91022f, 0.595f, 9.76f);
		GameObject.Find("CardboardBox_1").transform.position = new Vector3(-56.72f, 0.595f, 8.81f);
		GameObject.Find("CardboardBox_2").transform.position = new Vector3(-58.37f, 0.39f, 9.03f);

		m_Rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
	}

	private void SetObjectInPrac()
    {
		// Setting light in Practice
		SettingLight_City.LightLevel = 2;

		foreach (GameObject objectActiveInPrac in ObjectActivatedInPractice)
        {
			objectActiveInPrac.SetActive(true);
        }

		foreach (GameObject objectHideInPrac in ObjectHidedInPractice)
        {
			objectHideInPrac.SetActive(false);
        }

		GameObject.Find("Directional light").GetComponent<SettingBlockage_City>().enabled = false;
		GameObject.Find("Blockage 0206 - books").transform.position = new Vector3(-57f, 0.262f, 5.5f);
		GameObject.Find("Blockage 0301 - rusty_tricycle").transform.position = new Vector3(-38.25f, 0.25f, 7f);
		GameObject.Find("Blockage 0105 - FridgeOld").transform.position = new Vector3(-63f, 0.55f, 10f);
    }

	private void SetSavingPath()
	{
		SavePath = (SettingLight_City.LightLevel == 3) ? DataManager.dataSavingPath[3] : DataManager.dataSavingPath[4];

		if (isTutorial) filename = "City_Navigation" + "_Tutorial_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_SaveData.csv";
		else if (isRealTest)
		{
			filename = "City_Navigation" + "_RealTest_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_SaveData.csv";
		}
		else if (isPractice) filename = "City_Navigation" + "_Practice_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_SaveData.csv";
	}
	private void SetLanguage()
	{
		
		if (UserInfoLanguage.currentLanguage == "Mandarin")
		{
			InstructionAudioClip = ChineseAudios;
		}
	}


	private void ActionsTrigger()
    {
		if (isTutorial)
		{
			try
			{
				if (instruction_order == 0)
				{
					Action_Welcome(0, 20f);
				}

				if (instruction_order == 1)
				{
					Action_Introduction(1);
				}

				if (instruction_order == 2)
				{
					Action_ForwardTest(2, 15f);
				}

				if (instruction_order == 3)
				{
					Action_BackwardTest1(3);
				}

				if (instruction_order == 4)
				{
					//Action_BackwardTest2(4, 15f, 0.2f);
					Action_BackwardTest2(4, 15f, pedestrainCollisionPosX - 0.9f);
				}

				if (instruction_order == 5)
				{
					Action_BackwardTest3(5, 15f, lastPlayerPosX + 2f);
				}

				if (instruction_order == 6)
				{
					Action_PavementWalk1(6);
				}

				if (instruction_order == 7)
				{
					Action_PavementWalk2(7, 15f, "Obstacle1_Trigger");
				}

				if (instruction_order == 8)
				{
					Action_Obstacle1(8);
				}

				if (instruction_order == 9)
				{
					Action_Pedestrian1(9, 20f);
				}

				if (instruction_order == 10)
				{
					Action_Obstacle2(10);
				}

				if (instruction_order == 11)
				{
					Action_Pedestrian2(11, 20f, pedestrainCollisionPosX - 0.5f);
				}

				if (instruction_order == 12)
				{
					Action_Pedestrian3(12, 15f, lastPlayerPosX + 1.3f);
				}

				if (instruction_order == 13)
				{
					Action_BoxCollider1(13, 15f);
				}

				if (instruction_order == 14)
				{
					Action_BoxCollider2(14, 20f, pedestrainCollisionPosX - 0.7f);
				}

				if (instruction_order == 15)
				{
					Action_BoxCollider3(15, 15f, lastPlayerPosX + 1.5f);
				}

				if (instruction_order == 16)
				{
					Action_PavementWalk3(16, 30f, "CarReminder_Trigger");
				}

				if (instruction_order == 17)
				{
					Action_RemindTheCar(17);
				}

				if (instruction_order == 18)
				{
					Action_RoadCrossing(18, 100f, "LastSecond_Trigger");
				}

				if (instruction_order == 19)
				{
					Action_SecondLastCheckPoint(19, 20f, oldPosition.x + 66f);
				}

				if (instruction_order == 20)
				{
					Action_EndCheckPoint(20, 15f);
				}
			}
			catch (Exception e)
			{
				UnityEngine.Debug.Log(e.Message);
			}
		}
		else if (isPractice)
		{
			if (!Finished)
            {
				if (!isInstructionFinished && instruction_order == 21)
					PracticeAudio1(21, 15f);
			}
            else
            {
				PracticeAudio2(22, 15f);
			}			
		}
		else if (isRealTest)
			if (!isInstructionFinished && !Finished)
				RealTestAudio1(23);
	}

	private void Action_Welcome(int audio_order, float audioActivatedDuration)
	{
		instructionTimeElapsed += Time.deltaTime;

		if (instructionTimeElapsed <= audioActivatedDuration)
		{
			if (!isInstructionAudioActivated && !InstructionAudioSource.isPlaying)
			{
				InstructionAudioPlay(true, audio_order);
				ControllerButtonAcess(false, false);
			}

			else if (isInstructionAudioActivated && InstructionAudioSource.isPlaying)
			{
				ControllerButtonAcess(false, false);
			}

			else if (isInstructionAudioActivated && !InstructionAudioSource.isPlaying)
			{
				ControlerModel.SetActive(true);
				showForwardButton = true;

				ControllerButtonAcess(false, false);
				if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.KeypadEnter))
				{
					isInstructionFinished = true;
				}
			}
		}

		else
		{
			ControlerModel.SetActive(true);
			showForwardButton = true;

			//ControllerButtonAcess(true, false);
			ControllerButtonAcess(false, false);
			if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.KeypadEnter))
			{
				isInstructionFinished = true;
			}
			if (((int)instructionTimeElapsed) % ((int)audioActivatedDuration) == 0
				&& !InstructionAudioSource.isPlaying)
			{
				InstructionAudioPlay(true, audio_order);

				Flashing_City.isFlashing = false;
			}
		}

		if (isInstructionFinished)
		{
			DebugForActionOrder(audio_order);
			InstructionParametersInit();
			InstructionAudioPlay(false, audio_order);

			ControlerModel.SetActive(false);
			showForwardButton = false;
			Flashing_City.isFlashing = false;
		}
	}

	private void Action_Introduction(int audio_order)
	{

		if (!isInstructionAudioActivated && !InstructionAudioSource.isPlaying)
		{
			InstructionAudioPlay(true, audio_order);
			isInstructionAudioActivated = true;
		}

		else if (isInstructionAudioActivated && InstructionAudioSource.isPlaying)
		{
			ControllerButtonAcess(false, false);
		}

		else if (isInstructionAudioActivated && !InstructionAudioSource.isPlaying)
		{
			isInstructionFinished = true;
		}

		if (isInstructionFinished)
		{
			DebugForActionOrder(audio_order);
			InstructionParametersInit();
			InstructionAudioPlay(false, audio_order);
		}
	}

	private void Action_ForwardTest(int audio_order, float audioActivatedDuration)
	{
		instructionTimeElapsed += Time.deltaTime;

		if (instructionTimeElapsed <= audioActivatedDuration)
		{

			if (!isInstructionAudioActivated && !InstructionAudioSource.isPlaying)
			{
				InstructionAudioPlay(true, audio_order);
				ControllerButtonAcess(false, false);

			}

			else if (isInstructionAudioActivated && InstructionAudioSource.isPlaying)
			{
				ControllerButtonAcess(false, false);
			}

			else if (isInstructionAudioActivated && !InstructionAudioSource.isPlaying)
			{
				ControlerModel.SetActive(true);
				showForwardButton = true;

				ControllerButtonAcess(true, false);
				if (onCollisionObjectName == "StreetTable_Trigger")
				{
					pedestrainCollisionPosX = GameObject.Find("XRRig").transform.position.x;
					isInstructionFinished = true;
				}
			}
		}

		else
		{
			ControlerModel.SetActive(true);
			showForwardButton = true;

			ControllerButtonAcess(true, false);
			if (onCollisionObjectName == "StreetTable_Trigger")
			{
				pedestrainCollisionPosX = GameObject.Find("XRRig").transform.position.x;
				isInstructionFinished = true;
			}
			if (((int)instructionTimeElapsed) % ((int)audioActivatedDuration) == 0
				&& !InstructionAudioSource.isPlaying)
			{
				InstructionAudioPlay(true, audio_order);

				Flashing_City.isFlashing = false;
			}
		}

		if (isInstructionFinished)
		{
			GameObject.Find(onCollisionObjectName).SetActive(false);
			DebugForActionOrder(audio_order);
			InstructionParametersInit();
			InstructionAudioPlay(false, audio_order);

			GameObject.Find("Tutorial_Blockage_StreetTable").transform.GetChild(2).gameObject.SetActive(false);

			ControlerModel.SetActive(false);
			showForwardButton = false;
			Flashing_City.isFlashing = false;
		}
	}

	private void Action_BackwardTest1(int audio_order)
	{
		Action_Introduction(audio_order);
	}

	private void Action_BackwardTest2(int audio_order, float audioActivatedDuration, float backPosition)
	{
		instructionTimeElapsed += Time.deltaTime;

		if (instructionTimeElapsed <= audioActivatedDuration)
		{

			if (!isInstructionAudioActivated && !InstructionAudioSource.isPlaying)
			{
				InstructionAudioPlay(true, audio_order);
				ControllerButtonAcess(false, false);

			}

			else if (isInstructionAudioActivated && InstructionAudioSource.isPlaying)
			{
				ControllerButtonAcess(false, false);
			}

			else if (isInstructionAudioActivated && !InstructionAudioSource.isPlaying)
			{
				ControlerModel.SetActive(true);
				showBackButton = true;
				ControllerButtonAcess(false, true);
				isInstructionFinished = BackwardDistanceSatisfied(backPosition);
			}
		}

		else
		{
			ControlerModel.SetActive(true);
			showBackButton = true;
			ControllerButtonAcess(false, true);
			isInstructionFinished = BackwardDistanceSatisfied(backPosition);
			if (((int)instructionTimeElapsed) % ((int)audioActivatedDuration) == 0
				&& !InstructionAudioSource.isPlaying)
			{
				InstructionAudioPlay(true, audio_order);

				Flashing_City.isFlashing = false;
			}
		}

		if (isInstructionFinished)
		{
			DebugForActionOrder(audio_order);
			InstructionParametersInit();
			InstructionAudioPlay(false, audio_order);

			ControlerModel.SetActive(false);
			showBackButton = false;
			Flashing_City.isFlashing = false;
		}
	}

	private void Action_BackwardTest3(int audio_order, float audioActivatedDuration, float forwardPosition)
	{
		instructionTimeElapsed += Time.deltaTime;

		if (instructionTimeElapsed <= audioActivatedDuration)
		{

			if (!isInstructionAudioActivated && !InstructionAudioSource.isPlaying)
			{
				InstructionAudioPlay(true, audio_order);
				ControllerButtonAcess(false, false);

			}

			else if (isInstructionAudioActivated && InstructionAudioSource.isPlaying)
			{
				ControllerButtonAcess(false, false);
			}

			else if (isInstructionAudioActivated && !InstructionAudioSource.isPlaying)
			{
				ControllerButtonAcess(true, true);

				isInstructionFinished = ForwardDistanceSatisfied(forwardPosition);
			}
		}

		else
		{
			ControllerButtonAcess(true, true);

			isInstructionFinished = ForwardDistanceSatisfied(forwardPosition);
			if (((int)instructionTimeElapsed) % ((int)audioActivatedDuration) == 0
				&& !InstructionAudioSource.isPlaying)
			{
				InstructionAudioPlay(true, audio_order);
			}
		}

		if (isInstructionFinished)
		{
			DebugForActionOrder(audio_order);
			InstructionParametersInit();
			InstructionAudioPlay(false, audio_order);
		}
	}

	private void Action_PavementWalk1(int audio_order)
	{
		Action_Introduction(audio_order);
	}

	private void Action_PavementWalk2(int audio_order, float audioActivatedDuration, string collisionObject)
	{
		instructionTimeElapsed += Time.deltaTime;

		if (instructionTimeElapsed <= audioActivatedDuration)
		{

			if (!isInstructionAudioActivated && !InstructionAudioSource.isPlaying)
			{
				InstructionAudioPlay(true, audio_order);
				ControllerButtonAcess(false, false);

			}

			else if (isInstructionAudioActivated && InstructionAudioSource.isPlaying)
			{
				ControllerButtonAcess(false, false);
			}

			else if (isInstructionAudioActivated && !InstructionAudioSource.isPlaying)
			{
				ControllerButtonAcess(true, true);
				if (onCollisionObjectName == collisionObject)
				{
					isInstructionFinished = true;
				}
			}
		}

		else
		{
			ControllerButtonAcess(true, true);
			if (onCollisionObjectName == collisionObject)
			{
				isInstructionFinished = true;
			}
			if (((int)instructionTimeElapsed) % ((int)audioActivatedDuration) == 0
				&& !InstructionAudioSource.isPlaying)
			{
				InstructionAudioPlay(true, audio_order);
			}
		}

		if (isInstructionFinished)
		{
			GameObject.Find(onCollisionObjectName).SetActive(false);
			DebugForActionOrder(audio_order);
			InstructionParametersInit();
			InstructionAudioPlay(false, audio_order);
		}
	}
	
	private void Action_Obstacle1(int audio_order)
	{
		Action_Introduction(audio_order);
	}
	
	private void Action_Pedestrian1(int audio_order, float audioActivatedDuration)
	{
		instructionTimeElapsed += Time.deltaTime;

		if (instructionTimeElapsed <= audioActivatedDuration)
		{

			if (!isInstructionAudioActivated && !InstructionAudioSource.isPlaying)
			{
				InstructionAudioPlay(true, audio_order);
				ControllerButtonAcess(false, false);

			}

			else if (isInstructionAudioActivated && InstructionAudioSource.isPlaying)
			{
				ControllerButtonAcess(false, false);
			}

			else if (isInstructionAudioActivated && !InstructionAudioSource.isPlaying)
			{
				ControllerButtonAcess(true, true);
				if (onCollisionObjectName == "Tutorial_Blockage_Pedestrain")
				{
					pedestrainCollisionPosX = GameObject.Find("XRRig").transform.position.x;
					isInstructionFinished = true;
				}
			}
		}

		else
		{
			ControllerButtonAcess(true, true);
			if (onCollisionObjectName == "Tutorial_Blockage_Pedestrain")
			{
				pedestrainCollisionPosX = GameObject.Find("XRRig").transform.position.x;
				isInstructionFinished = true;
			}
			if (((int)instructionTimeElapsed) % ((int)audioActivatedDuration) == 0
				&& !InstructionAudioSource.isPlaying)
			{
				InstructionAudioPlay(true, audio_order);
			}
		}

		if (isInstructionFinished)
		{
			DebugForActionOrder(audio_order);
			InstructionParametersInit();
			InstructionAudioPlay(false, audio_order);

			GameObject.Find("Tutorial_Blockage_Pedestrain").GetComponent<PedestrianWalkingMCS002>().enabled = false;
			ArrowPedestrain.SetActive(false);
		}

		else
        {
			ArrowPedestrain.SetActive(true);
		}
	}

	private void Action_Obstacle2(int audio_order)
	{
		Action_Introduction(audio_order);
	}
	
	private void Action_Pedestrian2(int audio_order, float audioActivatedDuration, float backPosition)
	{
		instructionTimeElapsed += Time.deltaTime;

		if (instructionTimeElapsed <= audioActivatedDuration)
		{

			if (!isInstructionAudioActivated && !InstructionAudioSource.isPlaying)
			{
				InstructionAudioPlay(true, audio_order);
				ControllerButtonAcess(false, false);

			}

			else if (isInstructionAudioActivated && InstructionAudioSource.isPlaying)
			{
				ControllerButtonAcess(false, false);
			}

			else if (isInstructionAudioActivated && !InstructionAudioSource.isPlaying)
			{
				ControllerButtonAcess(false, true);
				isInstructionFinished = BackwardDistanceSatisfied(backPosition);
			}
		}

		else
		{
			
			ControllerButtonAcess(false, true);
			isInstructionFinished = BackwardDistanceSatisfied(backPosition);
			if (((int)instructionTimeElapsed) % ((int)audioActivatedDuration) == 0
				&& !InstructionAudioSource.isPlaying)
			{
				InstructionAudioPlay(true, audio_order);
			}
		}

		if (isInstructionFinished)
		{
			DebugForActionOrder(audio_order);
			InstructionParametersInit();
			InstructionAudioPlay(false, audio_order);
		}
	}
	
	private void Action_Pedestrian3(int audio_order, float audioActivatedDuration, float forwardPosition)
	{
		instructionTimeElapsed += Time.deltaTime;

		if (instructionTimeElapsed <= audioActivatedDuration)
		{

			if (!isInstructionAudioActivated && !InstructionAudioSource.isPlaying)
			{
				InstructionAudioPlay(true, audio_order);
				ControllerButtonAcess(false, false);
			}

			else if (isInstructionAudioActivated && InstructionAudioSource.isPlaying)
			{
				ControllerButtonAcess(false, false);
			}

			else if (isInstructionAudioActivated && !InstructionAudioSource.isPlaying)
			{
				ControllerButtonAcess(true, true);
				isInstructionFinished = ForwardDistanceSatisfied(forwardPosition);
			}
		}

		else
		{
			ControllerButtonAcess(true, true);
			isInstructionFinished = ForwardDistanceSatisfied(forwardPosition);
			
			if (((int)instructionTimeElapsed) % ((int) audioActivatedDuration) == 0
				&& !InstructionAudioSource.isPlaying)
			{
				InstructionAudioPlay(true, audio_order);
			}
		}

		if (isInstructionFinished)
		{
            if (audio_order ==12)
            {
				ArrowBox.SetActive(true);
            }
			DebugForActionOrder(audio_order);
			InstructionParametersInit();
			InstructionAudioPlay(false, audio_order);
		}
	}
	
	private void Action_BoxCollider1(int audio_order, float audioActivatedDuration)
	{
		instructionTimeElapsed += Time.deltaTime;

		if (instructionTimeElapsed <= audioActivatedDuration)
		{

			if (!isInstructionAudioActivated && !InstructionAudioSource.isPlaying)
			{
				InstructionAudioPlay(true, audio_order);
				ControllerButtonAcess(false, false);

			}

			else if (isInstructionAudioActivated && InstructionAudioSource.isPlaying)
			{
				ControllerButtonAcess(false, false);
			}

			else if (isInstructionAudioActivated && !InstructionAudioSource.isPlaying)
			{
				ControllerButtonAcess(true, true);
				if (onCollisionObjectName == "Tutorial_Blockage_BookBoxes")
				{
					pedestrainCollisionPosX = GameObject.Find("XRRig").transform.position.x;
					isInstructionFinished = true;
				}
			}
		}

		else
		{
			ControllerButtonAcess(true, true);
			if (onCollisionObjectName == "Tutorial_Blockage_BookBoxes")
			{
				pedestrainCollisionPosX = GameObject.Find("XRRig").transform.position.x;
				isInstructionFinished = true;
			}
			if (((int)instructionTimeElapsed) % ((int)audioActivatedDuration) == 0
				&& !InstructionAudioSource.isPlaying)
			{
				InstructionAudioPlay(true, audio_order);
			}
		}

		if (isInstructionFinished)
		{
			GameObject.Destroy(ArrowBox);

			DebugForActionOrder(audio_order);
			InstructionParametersInit();
			InstructionAudioPlay(false, audio_order);
		}
	}
	
	private void Action_BoxCollider2(int audio_order, float audioActivatedDuration, float backPosition)
	{
		Action_Pedestrian2(audio_order, audioActivatedDuration, backPosition);
	}
	
	private void Action_BoxCollider3(int audio_order, float audioActivatedDuration, float forwardPosition)
	{
		Action_Pedestrian3(audio_order, audioActivatedDuration, forwardPosition);
	}

	private void Action_PavementWalk3(int audio_order, float audioActivatedDuration, string collisionObject)
	{
		Action_PavementWalk2(audio_order, audioActivatedDuration, collisionObject);
	}

	private void Action_RemindTheCar(int audio_order)
	{
		Action_Introduction(audio_order);
	}

	private void Action_RoadCrossing(int audio_order, float audioActivatedDuration, string collisionObject)
	{
		Action_PavementWalk2(audio_order, audioActivatedDuration, collisionObject);
	}

	private void Action_SecondLastCheckPoint(int audio_order, float audioActivatedDuration, float forwardPosition)
	{
		Action_Pedestrian3(audio_order, audioActivatedDuration, forwardPosition);
	}

	private void Action_EndCheckPoint(int audio_order, float audioActivatedDuration)
	{
		if (Finished)
        {
			instructionTimeElapsed += Time.deltaTime;

			if (instructionTimeElapsed <= audioActivatedDuration)
			{
				if (!isInstructionAudioActivated && !InstructionAudioSource.isPlaying)
				{
					InstructionAudioPlay(true, audio_order);
					ControllerButtonAcess(false, false);
				}

				else if (isInstructionAudioActivated && InstructionAudioSource.isPlaying)
				{
					ControllerButtonAcess(false, false);
				}

				else if (isInstructionAudioActivated && !InstructionAudioSource.isPlaying)
				{
					ControllerButtonAcess(false, false);
					if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.KeypadEnter))
					{
						isInstructionFinished = true;
					}
				}
			}

			else
			{
				ControllerButtonAcess(true, false);
				if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.KeypadEnter))
				{
					isInstructionFinished = true;
				}
				if (((int)instructionTimeElapsed) % ((int)audioActivatedDuration) == 0
					&& !InstructionAudioSource.isPlaying)
				{
					InstructionAudioPlay(true, audio_order);
				}
			}

			if (isInstructionFinished)
			{
				DebugForActionOrder(audio_order);
				InstructionParametersInit();
				InstructionAudioPlay(false, audio_order);

				isChangingToPrac = true;
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			}
		}
		
	}

	private void InstructionAudioPlay(bool allowPlay, int instruction_order)
	{
		if (allowPlay)
		{
			InstructionAudioSource.clip = InstructionAudioClip[instruction_order];
			InstructionAudioSource.Play();
			isInstructionAudioActivated = true;
		}

		if (!allowPlay)
		{
			InstructionAudioSource.clip = InstructionAudioClip[instruction_order];
			InstructionAudioSource.Stop();
		}
	}

	private void ControllerButtonAcess(bool button_O, bool button_X)
	{
		controllerButton_Backward_X = button_X;
		controllerButton_Forward_O = button_O;
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (hit.gameObject.tag != "Blockage" && hit.gameObject.tag != "NPC" && hit.gameObject.tag != "Trigger" || hit.gameObject.name.Contains("Box"))
        {
			if (hit.gameObject.transform.parent.gameObject.tag == "Blockage" || hit.gameObject.transform.parent.gameObject.tag == "NPC")
            {
				onCollisionObjectName = hit.gameObject.transform.parent.gameObject.name;
			}
		}
        else
        {
			onCollisionObjectName = hit.gameObject.name;
		}
	}

	private bool KeyHoldTimeSatisfied(float requestKeyPeriod, KeyCode key)
	{
		if (Input.GetKey(key))
		{
			keyHoldTimeElapsed += Time.deltaTime;
		}
		if (Input.GetKeyUp(key))
		{
			if (keyHoldTimeElapsed >= requestKeyPeriod)
			{
				isKeyHoldTimeSatisfied = true;
				keyHoldTimeElapsed = 0f;
			}
			else
			{
				keyHoldTimeElapsed = 0f;
			}
		}
		return isKeyHoldTimeSatisfied;
	}

	private bool BackwardDistanceSatisfied(float backPosition)
    {
		if (GameObject.Find("XRRig").transform.position.x <= backPosition)
        {
			isBackwardDistanceSatisfied = true;
			lastPlayerPosX = GameObject.Find("XRRig").transform.position.x;
		}
		return isBackwardDistanceSatisfied;
    }

	private bool ForwardDistanceSatisfied(float forwardPosition)
	{
		if (GameObject.Find("XRRig").transform.position.x >= forwardPosition)
		{
			isForwardDistanceSatisfied = true;
		}
		return isForwardDistanceSatisfied;
	}

	private void InstructionParametersInit(bool audioActivated = false, bool finished = false, bool keyHold = false, float timer = 0f)
	{
		if (instruction_order != 20)
        {
			instruction_order++;
		}
        else
        {
			instruction_order = 0;
        }

		isInstructionAudioActivated = audioActivated;
		isInstructionFinished = finished;
		isKeyHoldTimeSatisfied = keyHold;
		instructionTimeElapsed = timer;
		onCollisionObjectName = "";

		isBackwardDistanceSatisfied = false;
		isForwardDistanceSatisfied = false;
	}

	private void DebugForActionOrder(int action_order)
	{
		UnityEngine.Debug.Log(actionsDebugList[action_order]);
	}

	private void PracticeAudio1(int audio_order, float audioActivatedDuration)
    {
		instructionTimeElapsed += Time.deltaTime;

		if (instructionTimeElapsed <= audioActivatedDuration)
		{
			if (!isInstructionAudioActivated && !InstructionAudioSource.isPlaying)
			{
				InstructionAudioPlay(true, audio_order);
				ControllerButtonAcess(false, false);
			}

			else if (isInstructionAudioActivated && InstructionAudioSource.isPlaying)
			{
				ControllerButtonAcess(false, false);
			}

			else if (isInstructionAudioActivated && !InstructionAudioSource.isPlaying)
			{
				ControllerButtonAcess(true, true);
				isInstructionFinished = true;
			}
		}

		else
		{
			ControllerButtonAcess(true, true);
			isInstructionFinished = true;

			if (((int)instructionTimeElapsed) % ((int)audioActivatedDuration) == 0
				&& !InstructionAudioSource.isPlaying)
			{
				InstructionAudioPlay(true, audio_order);
			}
		}

		if (isInstructionFinished)
		{
			InstructionAudioPlay(false, audio_order);
			instructionTimeElapsed = 0f;
			isInstructionFinished = false;
			isInstructionAudioActivated = false;
			instruction_order++;
		}
	}

	private void PracticeAudio2(int audio_order, float audioActivatedDuration)
    {
		instructionTimeElapsed += Time.deltaTime;

		if (instructionTimeElapsed <= audioActivatedDuration)
		{
			if (!isInstructionAudioActivated && !InstructionAudioSource.isPlaying)
			{
				InstructionAudioPlay(true, audio_order);
				ControllerButtonAcess(false, false);
			}

			else if (isInstructionAudioActivated && InstructionAudioSource.isPlaying)
			{
				ControllerButtonAcess(false, false);
			}

			else if (isInstructionAudioActivated && !InstructionAudioSource.isPlaying)
			{
				ControllerButtonAcess(false, false);
				if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.KeypadEnter))
				{
					isInstructionFinished = true;
				}
			}
		}
		else
		{
			ControllerButtonAcess(true, true);
			if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.KeypadEnter))
			{
				isInstructionFinished = true;
			}
			if (((int)instructionTimeElapsed) % ((int)audioActivatedDuration) == 0
				&& !InstructionAudioSource.isPlaying)
			{
				InstructionAudioPlay(true, audio_order);
			}
		}
		if (isInstructionFinished)
		{
			InstructionAudioPlay(false, audio_order);

			isChangingToReal = true;
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}

	private void RealTestAudio1(int audio_order)
    {
		InstructionAudioPlay(true, audio_order);
		isInstructionFinished = true;
    }
}
