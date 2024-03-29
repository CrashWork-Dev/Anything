﻿#if UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS || UNITY_WSA || UNITY_WEBGL
namespace Net.UnityComponent
{
    using global::System;
    using Net.Component;
    using Net.Share;
    using Net.Client;
    using UnityEngine;

    /// <summary>
    /// 网络Transform同步组件基类
    /// </summary>
    [DefaultExecutionOrder(1000)]
    public class NetworkTransformMulti : NetworkTransformBase
    {
        public ChildTransform[] childs;

        public override void Start()
        {
            base.Start();
            InitChilds(); //实例化后再赋值位置时再次初始化位置用到
        }

        public override void OnNetworkObjectCreate(in Operation opt)
        {
            base.OnNetworkObjectCreate(opt);
            InitChilds(); //实例化后就要初始化子物体信息, 否则会出现子物体的大小变成0,0,0的问题
        }

        private void InitChilds()
        {
            for (int i = 0; i < childs.Length; i++)
            {
                childs[i].Init(i + 1);
            }
        }

        public override void NetworkUpdate()
        {
            if (netObj.Identity == -1 | currMode == SyncMode.None)
                return;
            if (currMode == SyncMode.Synchronized)
            {
                SyncTransform();
                for (int i = 0; i < childs.Length; i++)
                {
                    childs[i].SyncTransform();
                }
            }
            else if (Time.time > sendTime)
            {
                Check();
                for (int i = 0; i < childs.Length; i++)
                {
                    childs[i].Check(netObj.Identity, netObj.registerObjectIndex, NetComponentID);
                }
                sendTime = Time.time + (1f / rate);
            }
        }

        public override void StartSyncTransformState()
        {
            NetworkSceneManager.Instance.AddOperation(new Operation(Command.Transform, netObj.Identity, syncScale ? localScale : Net.Vector3.zero, syncPosition ? position : Net.Vector3.zero, syncRotation ? rotation : Net.Quaternion.zero)
            {
                cmd1 = (byte)currMode,
                index = netObj.registerObjectIndex,
                index1 = NetComponentID,
                uid = ClientBase.Instance.UID
            });
        }

        public override void OnInitialSynchronization(in Operation opt)
        {
            SetNetworkSyncState(opt);
            SetNetworkSyncMode(opt);
            SyncControlTransform();
        }

        public override void OnNetworkOperationHandler(in Operation opt)
        {
            if (ClientBase.Instance.UID == opt.uid)
                return;
            if (opt.index2 == 0)
            {
                SetNetworkSyncState(opt);
                if (currMode == SyncMode.SynchronizedAll | currMode == SyncMode.Control)
                    SyncControlTransform();
                else if (currMode == SyncMode.None)
                    SetNetworkSyncMode(opt);
            }
            else
            {
                sendTime = Time.time + interval;
                var child = childs[opt.index2 - 1];
                child.netPosition = opt.position;
                child.netRotation = opt.rotation;
                child.netLocalScale = opt.direction;
                if (child.mode == SyncMode.SynchronizedAll | child.mode == SyncMode.Control)
                    child.SyncControlTransform();
            }
        }
    }

    [Serializable]
    public class ChildTransform
    {
        public string name;
        public Transform transform;
        internal Net.Vector3 position;
        internal Net.Quaternion rotation;
        internal Net.Vector3 localScale;
        public SyncMode mode = SyncMode.Control;
        public bool syncPosition = true;
        public bool syncRotation = true;
        public bool syncScale = false;
        public int identity = -1;//自身id
        internal Net.Vector3 netPosition;
        internal Net.Quaternion netRotation;
        internal Net.Vector3 netLocalScale;

        internal void Init(int identity)
        {
            this.identity = identity;
            netPosition = position = transform.localPosition;
            netRotation = rotation = transform.localRotation;
            netLocalScale = localScale = transform.localScale;
        }

        internal void Check(int identity, int index, int netIndex)
        {
            if (transform.localPosition != position | transform.localRotation != rotation | transform.localScale != localScale)
            {
                position = transform.localPosition;
                rotation = transform.localRotation;
                localScale = transform.localScale;
                NetworkSceneManager.Instance.AddOperation(new Operation(Command.Transform, identity, syncScale ? localScale : Net.Vector3.zero, syncPosition ? position : Net.Vector3.zero, syncRotation ? rotation : Net.Quaternion.zero)
                {
                    cmd1 = (byte)mode,
                    uid = ClientBase.Instance.UID,
                    index = index,
                    index1 = netIndex,
                    index2 = this.identity
                });
            }
        }

        public void SyncTransform()
        {
            if (syncPosition)
                transform.localPosition = Vector3.Lerp(transform.localPosition, netPosition, 0.3f);
            if (syncRotation)
                if (netRotation != Net.Quaternion.identity)
                    transform.localRotation = Quaternion.Lerp(transform.localRotation, netRotation, 0.3f);
            if (syncScale)
                transform.localScale = netLocalScale;
        }

        public void SyncControlTransform()
        {
            if (syncPosition)
                transform.localPosition = netPosition;
            if (syncRotation)
                transform.localRotation = netRotation;
            if (syncScale)
                transform.localScale = netLocalScale;
        }
    }
}
#endif