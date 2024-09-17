using Fbs;
using Google.FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserBuildingNetworkHelper
{
    #region Building Operation
    public static byte[] CreateFBAddBuildingRequestBody(AddBuildingRequestBody body)
    {
        FlatBufferBuilder builder = new(128);
        RequestBuildBuildingTownMap.StartRequestBuildBuildingTownMap(builder);

        RequestBuildBuildingTownMap.AddIdBuildingTemplate(builder, body.buildingId);
        RequestBuildBuildingTownMap.AddIdTile(builder, body.location);
        RequestBuildBuildingTownMap.AddRotation(builder, body.rotation);

        var offset = RequestBuildBuildingTownMap.EndRequestBuildBuildingTownMap(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }

    public static byte[] CreateFBRotateBuildingRequestBody(int buildingObjectId, int rotation)
    {
        FlatBufferBuilder builder = new(128);
        RequestDataRotateBuilding.StartRequestDataRotateBuilding(builder);

        RequestDataRotateBuilding.AddIdBuildingPlayer(builder, buildingObjectId);
        RequestDataRotateBuilding.AddRotation(builder, rotation);

        var offset = RequestDataRotateBuilding.EndRequestDataRotateBuilding(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }

    public static byte[] CreateFBStartConstructionRequestBody(int buildingObjectId)
    {
        FlatBufferBuilder builder = new(128);
        RequestStartBuildProgress.StartRequestStartBuildProgress(builder);

        RequestStartBuildProgress.AddIdBuildingPlayer(builder, buildingObjectId);

        var offset = RequestStartBuildProgress.EndRequestStartBuildProgress(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
    public static byte[] CreateFBCompleteConstrucionRequestBody(int buildingObjectId)
    {
        FlatBufferBuilder builder = new(128);
        RequestBuildComplete.StartRequestBuildComplete(builder);

        RequestBuildComplete.AddIdBuildingPlayer(builder, buildingObjectId);

        var offset = RequestBuildComplete.EndRequestBuildComplete(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
    public static byte[] CreateFBCancelConstructionRequestBody(int buildingObjectId)
    {
        FlatBufferBuilder builder = new(128);
        RequestCancelBuildBuilding.StartRequestCancelBuildBuilding(builder);

        RequestCancelBuildBuilding.AddBuildingPlayerId(builder, buildingObjectId);

        var offset = RequestCancelBuildBuilding.EndRequestCancelBuildBuilding(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }

    public static byte[] CreateFBUpgradeRequestBody(int buildingObjectId)
    {
        FlatBufferBuilder builder = new(128);
        UpgradeBuildingRequest.StartUpgradeBuildingRequest(builder);

        UpgradeBuildingRequest.AddBuildingPlayerId(builder, buildingObjectId);

        var offset = UpgradeBuildingRequest.EndUpgradeBuildingRequest(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
    public static byte[] CreateFBStartUpgradingRequestBody(int buildingObjectId)
    {
        FlatBufferBuilder builder = new(128);
        RequestStartProgressUpgrading.StartRequestStartProgressUpgrading(builder);

        RequestStartProgressUpgrading.AddIdBuildingPlayer(builder, buildingObjectId);

        var offset = RequestStartProgressUpgrading.EndRequestStartProgressUpgrading(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
    public static byte[] CreateFBCompleteUpgradeRequestBody(int buildingObjectId)
    {
        FlatBufferBuilder builder = new(128);
        RequestCompleteUpgrade.StartRequestCompleteUpgrade(builder);

        RequestCompleteUpgrade.AddIdBuildingPlayer(builder, buildingObjectId);

        var offset = RequestCompleteUpgrade.EndRequestCompleteUpgrade(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
    public static byte[] CreateFBCancelUpgradeRequestBody(int buildingObjectId)
    {
        FlatBufferBuilder builder = new(128);
        RequestCancelUpgradeBuilding.StartRequestCancelUpgradeBuilding(builder);

        RequestCancelUpgradeBuilding.AddIdBuildingPlayer(builder, buildingObjectId);

        var offset = RequestCancelUpgradeBuilding.EndRequestCancelUpgradeBuilding(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }

    public static byte[] CreateFBStartDestroyRequestBody(int buildingObjectId)
    {
        FlatBufferBuilder builder = new(128);
        RequestDestroyBuilding.StartRequestDestroyBuilding(builder);

        RequestDestroyBuilding.AddIdBuildingPlayer(builder, buildingObjectId);

        var offset = RequestDestroyBuilding.EndRequestDestroyBuilding(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
    public static byte[] CreateFBFinishDestroyRequestBody(int buildingObjectId)
    {
        FlatBufferBuilder builder = new(128);
        RequestCompleteDestroyBuilding.StartRequestCompleteDestroyBuilding(builder);

        RequestCompleteDestroyBuilding.AddIdBuildingPlayer(builder, buildingObjectId);

        var offset = RequestCompleteDestroyBuilding.EndRequestCompleteDestroyBuilding(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
    public static byte[] CreateFBCancelDestroyRequestBody(int buildingObjectId)
    {
        FlatBufferBuilder builder = new(128);
        RequestCancelDestroyBuilding.StartRequestCancelDestroyBuilding(builder);

        RequestCancelDestroyBuilding.AddIdBuildingPlayer(builder, buildingObjectId);

        var offset = RequestCancelDestroyBuilding.EndRequestCancelDestroyBuilding(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
    public static byte[] CreateFBCompletelyRemoveRequestBody(int buildingObjectId)
    {
        FlatBufferBuilder builder = new(128);
        RequestFinalDestroyBuilding.StartRequestFinalDestroyBuilding(builder);

        RequestFinalDestroyBuilding.AddIdBuildingPlaye(builder, buildingObjectId);

        var offset = RequestFinalDestroyBuilding.EndRequestFinalDestroyBuilding(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }

    public static byte[] CreateFBGetUserBuildingBody(int buildingObjectId)
    {
        FlatBufferBuilder builder = new(128);
        GetInfoBuildingInTownMap.StartGetInfoBuildingInTownMap(builder);

        GetInfoBuildingInTownMap.AddIdBuildingPlayer(builder, buildingObjectId);

        var offset = GetInfoBuildingInTownMap.EndGetInfoBuildingInTownMap(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
    #endregion // Building Operation

    #region Building Production
    public static byte[] CreateQueueAProductRequestBody(int buildingObjId, UserBuildingProduction.QueuedProduct product)
    {
        FlatBufferBuilder builder = new(2);
        var queuedproduct = CreateFbQueuedProduct(builder, product);
        var offset = RequestQueueProduct.CreateRequestQueueProduct(builder, buildingObjId, queuedproduct);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
    private static Offset<ItemQueue> CreateFbQueuedProduct(FlatBufferBuilder builder, UserBuildingProduction.QueuedProduct product)
    {
        int matCount = product.materialStar.Count;
        Offset<ItemMaterialStar>[] itemMatStar = new Offset<ItemMaterialStar>[matCount];
        for (int i = 0; i < matCount; i++)
        {
            var matStar = product.materialStar[i];

            var starOffset = MaterialStar.CreateMaterialStar(builder, matStar.stars[0], matStar.stars[1], matStar.stars[2], matStar.stars[3]);
            var itemMatStarOffset = ItemMaterialStar.CreateItemMaterialStar(builder, matStar.itemId, starOffset);
            itemMatStar[i] = itemMatStarOffset;
        }

        var itemMatStarOffsetVector = ItemQueue.CreateItemMaterialStarQueueVector(builder, itemMatStar);
        var queuedItemOffset = ItemQueue.CreateItemQueue(builder, product.productId, itemMatStarOffsetVector);

        return queuedItemOffset;
    }

    public static byte[] CreateRepeatQueueRequestBody(int buildingObjId, bool repeat)
    {
        // TODO RK add repeat
        FlatBufferBuilder builder = new(5);
        RequestRepeateQueue.StartRequestRepeateQueue(builder);

        RequestRepeateQueue.AddIdBuildingPlayer(builder, buildingObjId);
        RequestRepeateQueue.AddRepeat(builder, repeat);

        var offset = RequestRepeateQueue.EndRequestRepeateQueue(builder);

        builder.Finish(offset.Value);
        return builder.SizedByteArray();
    }

    public static byte[] CreateChangeQueueOrderRequestBody(int buildingObjId, List<UserBuildingProduction.QueuedProduct> queue)
    {
        FlatBufferBuilder builder = new(32);
        var productQueue = CreateFbProductQueue(builder, queue);
        var queuedProdctOffsetVector = RequestChangeQueueIndex.CreateRequestChangeItemQueueVector(builder, productQueue);

        RequestChangeQueueIndex.StartRequestChangeQueueIndex(builder);

        RequestChangeQueueIndex.AddIdBuildingPlayer(builder, buildingObjId);
        RequestChangeQueueIndex.AddRequestChangeItemQueue(builder, queuedProdctOffsetVector);

        var offset = RequestChangeQueueIndex.EndRequestChangeQueueIndex(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
    private static Offset<ItemQueue>[] CreateFbProductQueue(FlatBufferBuilder builder, List<UserBuildingProduction.QueuedProduct> queue)
    {
        var itemQueue = new Offset<ItemQueue>[queue.Count];
        for (int i = 0; i < queue.Count; i++)
        {
            var productOffset = CreateFbQueuedProduct(builder, queue[i]);
            itemQueue[i] = productOffset;
        }

        return itemQueue;
    }

    public static byte[] CreateCancelCurrentProductionRequestBody(int buildingObjId)
    {
        FlatBufferBuilder builder = new(4);
        RequestCancelProduct.StartRequestCancelProduct(builder);

        RequestCancelProduct.AddIdBuildingPlayer(builder, buildingObjId);

        var offset = RequestCancelProduct.EndRequestCancelProduct(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }

    public static byte[] CreateCompleteProductRequestBody(int buildingObjId)
    {
        FlatBufferBuilder builder = new(4);
        RequestCompleteProduct.StartRequestCompleteProduct(builder);

        RequestCompleteProduct.AddIdBuilidngPlayer(builder, buildingObjId);

        var offset = RequestCompleteProduct.EndRequestCompleteProduct(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }

    public static byte[] CreateRemoveQueuedItemRequestBody(int buildingObjId, int queueIndex)
    {
        FlatBufferBuilder builder = new(8);
        RequestRemoveQueue.StartRequestRemoveQueue(builder);

        RequestRemoveQueue.AddIdBuildingPlayer(builder, buildingObjId);
        RequestRemoveQueue.AddIndex(builder, queueIndex);

        var offset = RequestRemoveQueue.EndRequestRemoveQueue(builder);
        builder.Finish(offset.Value);

        return builder.SizedByteArray();
    }
    #endregion
}

public class AddBuildingRequestBody
{
    public int buildingId;
    public int location;
    public int rotation;
}
