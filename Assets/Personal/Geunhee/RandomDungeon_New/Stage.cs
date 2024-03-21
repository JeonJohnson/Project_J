using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using AYellowpaper.SerializedCollections;


public class Stage : MonoBehaviour
{
	[ReadOnly]
	public int curRoomIndex;
	[ReadOnly]
	public Room curRoom;
	[ReadOnly]
	public List<Room> rooms;
	public List<Room> GetRoom(RoomType type)
	{
		return rooms.FindAll(x => x.Spec.roomType == type);
	}

	[Space(10f)]
	public SerializedDictionary<RoomType, GameObject> PortalPrefab;
	private Portal[] portals = new Portal[(int)RoomType.End];

	public void Initialize()
	{
		#region Setup_Portal
		for (int i = 0; i < (int)RoomType.End; ++i)
		{
			if (PortalPrefab[(RoomType)i] == null)
			{
				continue;
			}
			portals[i] = Instantiate(PortalPrefab[(RoomType)i]).GetComponent<Portal>();
			portals[i].gameObject.SetActive(false);
		}
		#endregion
	}

	public virtual void MoveNextRoom()
	{
		//포탈 밟고나서 연출 끝난 후 호출요망
		curRoom.Cleanup();
		curRoom.Disappear();
		
		curRoom.gameObject.SetActive(false);

		curRoomIndex += 1;
		curRoom = rooms[curRoomIndex];
		
		curRoom.gameObject.SetActive(true);
		curRoom.Appear();

		//FadeInOut 연출 해준뒤
		//인게임 컨트롤러든 어디서든 플레이어 등장 연출 추가 요망.
		//여기선 방만 컨트롤
		//자세한 연출은 노션 참고 또는 근희에게 말해주셈
	}


	public void GeneratingPortal()
	{
		Room nextRoom = rooms[curRoomIndex + 1];

		portals[(int)nextRoom.Spec.roomType].Appear();
	}
	

	private void Awake()
	{
		rooms = new List<Room>();	

		
	}
}
