﻿using WowPacketParser.Enums;
using WowPacketParser.Misc;
using WowPacketParser.Parsing;

namespace WowPacketParserModule.V6_0_2_19033.Parsers
{
    public static class BattlePayHandler
    {
        [Parser(Opcode.CMSG_BATTLE_PAY_GET_PRODUCT_LIST_QUERY)]
        [Parser(Opcode.CMSG_BATTLE_PAY_GET_PURCHASE_LIST_QUERY)]
        public static void HandleZeroLengthPackets(Packet packet)
        {
        }

        private static void ReadBattlepayDisplayInfo(ref Packet packet, params object[] indexes)
        {
            packet.ResetBitReader();

            var bit4 = packet.ReadBit("HasCreatureDisplayInfoID", Packet.GetIndexString(indexes));
            var bit12 = packet.ReadBit("HasFileDataID", Packet.GetIndexString(indexes));

            var bits16 = packet.ReadBits(10);
            var bits529 = packet.ReadBits(10);
            var bits1042 = packet.ReadBits(13);

            var bit5144 = packet.ReadBit("HasFlags", Packet.GetIndexString(indexes));

            if (bit4)
                packet.ReadInt32("CreatureDisplayInfoID", Packet.GetIndexString(indexes));

            if (bit12)
                packet.ReadInt32("FileDataID", Packet.GetIndexString(indexes));

            packet.ReadWoWString("Name1", bits16, Packet.GetIndexString(indexes));
            packet.ReadWoWString("Name2", bits529, Packet.GetIndexString(indexes));
            packet.ReadWoWString("Name3", bits1042, Packet.GetIndexString(indexes));

            if (bit5144)
                packet.ReadInt32("Flags", Packet.GetIndexString(indexes));
        }

        private static void ReadBattlePayProduct(ref Packet packet, params object[] indexes)
        {
            packet.ReadInt32("ProductID", Packet.GetIndexString(indexes));

            packet.ReadInt64("NormalPriceFixedPoint", Packet.GetIndexString(indexes));
            packet.ReadInt64("CurrentPriceFixedPoint", Packet.GetIndexString(indexes));

            var int11 = packet.ReadInt32("BattlepayProductItemCount", Packet.GetIndexString(indexes));

            packet.ReadByte("Type", Packet.GetIndexString(indexes));
            packet.ReadInt32("Flags", Packet.GetIndexString(indexes));

            for (int j = 0; j < int11; j++)
            {
                packet.ReadInt32("ID", Packet.GetIndexString(indexes), j);
                packet.ReadInt32("ItemID", Packet.GetIndexString(indexes), j);
                packet.ReadInt32("Quantity", Packet.GetIndexString(indexes), j);

                packet.ResetBitReader();

                var bit5160 = packet.ReadBit("HasBattlepayDisplayInfo", Packet.GetIndexString(indexes), j);
                packet.ReadBit("HasPet", Packet.GetIndexString(indexes), j);
                var bit5172 = packet.ReadBit("HasBATTLEPETRESULT", Packet.GetIndexString(indexes), j);

                if (bit5172)
                    packet.ReadBits("PetResult", 4);

                if (bit5160)
                    ReadBattlepayDisplayInfo(ref packet, indexes);
            }

            packet.ResetBitReader();

            packet.ReadBits("ChoiceType", 2, Packet.GetIndexString(indexes));

            var bit5196 = packet.ReadBit("HasBattlepayDisplayInfo", Packet.GetIndexString(indexes));
            if (bit5196)
                ReadBattlepayDisplayInfo(ref packet, Packet.GetIndexString(indexes));
        }

        private static void ReadBattlePayDistributionObject(ref Packet packet, params object[] indexes)
        {
            packet.ReadInt64("DistributionID", indexes);

            packet.ReadInt32("Status", indexes);
            packet.ReadInt32("ProductID", indexes);

            packet.ReadPackedGuid128("TargetPlayer", indexes);
            packet.ReadInt32("TargetVirtualRealm", indexes);
            packet.ReadInt32("TargetNativeRealm", indexes);

            packet.ReadInt64("PurchaseID", indexes);

            packet.ResetBitReader();

            var bit5248 = packet.ReadBit("HasBattlePayProduct", indexes);

            packet.ReadBit("Revoked", indexes);

            if (bit5248)
                ReadBattlePayProduct(ref packet, indexes);
        }

        private static void ReadBattlePayPurchase(Packet packet, params object[] indexes)
        {
            packet.ReadInt64("PurchaseID", indexes);
            packet.ReadInt32("Status", indexes);
            packet.ReadInt32("ResultCode", indexes);
            packet.ReadInt32("ProductID", indexes);

            packet.ResetBitReader();

            var bits20 = packet.ReadBits(8);
            packet.ReadWoWString("WalletName", bits20, indexes);
        }

        [Parser(Opcode.SMSG_BATTLE_PAY_GET_PURCHASE_LIST_RESPONSE)]
        public static void HandleBattlePayGetPurchaseListResponse(Packet packet)
        {
            packet.ReadUInt32("Result");

            var int6 = packet.ReadUInt32("BattlePayPurchaseCount");

            for (int i = 0; i < int6; i++)
                ReadBattlePayPurchase(packet, i);
        }

        [Parser(Opcode.SMSG_BATTLE_PAY_GET_DISTRIBUTION_LIST_RESPONSE)]
        public static void HandleBattlePayGetDistributionListResponse(Packet packet)
        {
            packet.ReadUInt32("Result");

            var int6 = packet.ReadUInt32("BattlePayDistributionObjectCount");

            for (uint index = 0; index < int6; index++)
                ReadBattlePayDistributionObject(ref packet, index);
        }

        [Parser(Opcode.SMSG_BATTLE_PAY_DISTRIBUTION_UPDATE)]
        public static void HandleBattlePayDistributionUpdate(Packet packet)
        {
            ReadBattlePayDistributionObject(ref packet);
        }

        [Parser(Opcode.SMSG_BATTLE_PAY_GET_PRODUCT_LIST_RESPONSE)]
        public static void HandletBattlePayGetProductListResponse(Packet packet)
        {
            packet.ReadUInt32("Result");
            packet.ReadUInt32("CurrencyID");

            var int52 = packet.ReadUInt32("BattlePayDistributionObjectCount");
            var int36 = packet.ReadUInt32("BattlePayProductGroupCount");
            var int20 = packet.ReadUInt32("BattlePayShopEntryCount");

            for (uint index = 0; index < int52; index++)
                ReadBattlePayProduct(ref packet, index);

            for (int i = 0; i < int36; i++)
            {
                packet.ReadInt32("GroupID", i);
                packet.ReadInt32("IconFileDataID", i);
                packet.ReadByte("DisplayType", i);
                packet.ReadInt32("Ordering", i);

                packet.ResetBitReader();

                var bits4 = packet.ReadBits(8);
                packet.ReadWoWString("Name", bits4, i);
            }

            for (uint i = 0; i < int20; i++)
            {
                packet.ReadUInt32("EntryID", i);
                packet.ReadUInt32("GroupID", i);
                packet.ReadUInt32("ProductID", i);
                packet.ReadInt32("Ordering", i);
                packet.ReadInt32("Flags", i);
                packet.ReadByte("BannerType", i);

                packet.ResetBitReader();

                var bit5172 = packet.ReadBit("HasBattlepayDisplayInfo", i);
                if (bit5172)
                    ReadBattlepayDisplayInfo(ref packet, i);
            }
        }

        [Parser(Opcode.CMSG_BATTLE_PAY_START_PURCHASE)]
        public static void HandleBattlePayStartPurchase(Packet packet)
        {
            packet.ReadInt32("ClientToken");
            packet.ReadInt32("ProductID");
            packet.ReadPackedGuid128("TargetCharacter");
        }

        [Parser(Opcode.SMSG_BATTLE_PAY_START_PURCHASE_RESPONSE)]
        public static void HandleBattlePayStartPurchaseResponse(Packet packet)
        {
            packet.ReadUInt64("PurchaseID");
            packet.ReadInt32("ClientToken");
            packet.ReadInt32("PurchaseResult");
        }

        [Parser(Opcode.SMSG_BATTLE_PAY_PURCHASE_UPDATE)]
        public static void HandleBattlePayPurchaseUpdate(Packet packet)
        {

            var battlePayPurchaseCount = packet.ReadUInt32("BattlePayPurchaseCount");
            for (int i = 0; i < battlePayPurchaseCount; i++)
                ReadBattlePayPurchase(packet, i);
        }

        [Parser(Opcode.SMSG_BATTLE_PAY_CONFIRM_PURCHASE)]
        public static void HandleBattlePayConfirmPurchase(Packet packet)
        {
            packet.ReadInt64("PurchaseID");
            packet.ReadInt64("CurrentPriceFixedPoint");
            packet.ReadInt32("ServerToken");
        }

        [Parser(Opcode.CMSG_BATTLE_PAY_CONFIRM_PURCHASE_RESPONSE)]
        public static void HandleBattlePayConfirmPurchaseResponse(Packet packet)
        {
            packet.ReadBit("ConfirmPurchase");
            packet.ReadInt32("ServerToken");
            packet.ReadInt64("ClientCurrentPriceFixedPoint");
        }

        [Parser(Opcode.SMSG_BATTLE_PAY_DELIVERY_ENDED)]
        public static void HandleBattlePayDeliveryEnded(Packet packet)
        {
            packet.ReadInt64("DistributionID");

            var itemCount = packet.ReadInt32("ItemCount");
            for (int i = 0; i < itemCount; i++)
                ItemHandler.ReadItemInstance(packet, i);
        }
    }
}
