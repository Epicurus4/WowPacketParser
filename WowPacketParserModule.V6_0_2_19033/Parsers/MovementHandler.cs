﻿using System;
using WowPacketParser.Enums;
using WowPacketParser.Misc;
using WowPacketParser.Parsing;

namespace WowPacketParserModule.V6_0_2_19033.Parsers
{
    public static class MovementHandler
    {
        public static void ReadMovementStats(Packet packet)
        {
            packet.ReadPackedGuid128("MoverGUID");

            packet.ReadUInt32("MoveIndex");
            packet.ReadVector4("Position");

            packet.ReadSingle("Pitch");
            packet.ReadSingle("StepUpStartElevation");

            var int152 = packet.ReadInt32("RemoveForcesCount");
            packet.ReadInt32("MoveTime");

            for (var i = 0; i < int152; i++)
                packet.ReadPackedGuid128("RemoveForcesIDs", i);

            packet.ResetBitReader();

            packet.ReadEnum<MovementFlag>("Movement Flags", 30);
            packet.ReadEnum<MovementFlagExtra>("Extra Movement Flags", 15);

            var hasTransport = packet.ReadBit("Has Transport Data");
            var hasFall = packet.ReadBit("Has Fall Data");
            packet.ReadBit("HasSpline");
            packet.ReadBit("HeightChangeFailed");
            packet.ReadBit("RemoteTimeValid");

            if (hasTransport)
            {
                packet.ReadPackedGuid128("TransportGuid");
                packet.ReadVector4("TransportPosition");
                packet.ReadByte("TransportSeat");
                packet.ReadInt32("TransportMoveTime");

                packet.ResetBitReader();

                var hasPrevMoveTime = packet.ReadBit("HasPrevMoveTime");
                var hasVehicleRecID = packet.ReadBit("HasVehicleRecID");

                if (hasPrevMoveTime)
                    packet.ReadUInt32("PrevMoveTime");

                if (hasVehicleRecID)
                    packet.ReadUInt32("VehicleRecID");
            }

            if (hasFall)
            {
                packet.ReadUInt32("FallTime");
                packet.ReadSingle("JumpVelocity");

                packet.ResetBitReader();

                var bit20 = packet.ReadBit("HasFallDirection");
                if (bit20)
                {
                    packet.ReadVector2("Direction");
                    packet.ReadSingle("HorizontalSpeed");
                }
            }
        }

        public static void ReadMovementAck(ref Packet packet)
        {
            ReadMovementStats(packet);
            packet.ReadInt32("AckIndex");
        }
        public static void ReadMovementForce(Packet packet)
        {
            packet.ReadPackedGuid128("ID");
            packet.ReadVector3("Direction");
            packet.ReadInt32("TransportID");
            packet.ReadSingle("Magnitude");

            packet.ResetBitReader();

            packet.ReadBits("Type", 2);
        }

        [Parser(Opcode.CMSG_MOVE_WORLDPORT_ACK)]
        public static void HandleZeroLengthPackets(Packet packet)
        {
        }

        [Parser(Opcode.SMSG_LOGIN_VERIFY_WORLD)]
        public static void HandleLoginVerifyWorld(Packet packet)
        {
            WowPacketParser.Parsing.Parsers.MovementHandler.CurrentMapId = (uint)packet.ReadEntry<Int32>(StoreNameType.Map, "Map");
            packet.ReadVector4("Position");
            packet.ReadUInt32("Reason");
        }

        [HasSniffData]
        [Parser(Opcode.SMSG_NEW_WORLD)]
        public static void HandleNewWorld(Packet packet)
        {
            WowPacketParser.Parsing.Parsers.MovementHandler.CurrentMapId = (uint)packet.ReadEntry<Int32>(StoreNameType.Map, "Map");
            packet.ReadVector4("Position");
            packet.ReadUInt32("Reason");

            packet.AddSniffData(StoreNameType.Map, (int)WowPacketParser.Parsing.Parsers.MovementHandler.CurrentMapId, "NEW_WORLD");
        }

        [Parser(Opcode.SMSG_LOGIN_SETTIMESPEED)]
        public static void HandleLoginSetTimeSpeed(Packet packet)
        {
            packet.ReadPackedTime("ServerTime");
            packet.ReadPackedTime("GameTime");
            packet.ReadSingle("NewSpeed");
            packet.ReadInt32("ServerTimeHolidayOffset");
            packet.ReadInt32("GameTimeHolidayOffset");
        }

        [Parser(Opcode.SMSG_GAME_TIME_UPDATE)]
        public static void HandleGameTimeUpdate(Packet packet)
        {
            packet.ReadPackedTime("ServerTime");
            packet.ReadPackedTime("GameTime");
            packet.ReadInt32("ServerTimeHolidayOffset");
            packet.ReadInt32("GameTimeHolidayOffset");
        }

        [Parser(Opcode.CMSG_MOVE_TIME_SKIPPED)]
        public static void HandleMoveTimeSkipped(Packet packet)
        {
            packet.ReadPackedGuid128("GUID");
            packet.ReadInt32("Time");
        }

        [Parser(Opcode.CMSG_MOVE_CHANGE_TRANSPORT)]
        [Parser(Opcode.CMSG_MOVE_FALL_LAND)]
        [Parser(Opcode.CMSG_MOVE_FALL_RESET)]
        [Parser(Opcode.CMSG_MOVE_HEARTBEAT)]
        [Parser(Opcode.CMSG_MOVE_JUMP)]
        [Parser(Opcode.CMSG_MOVE_SET_FACING)]
        [Parser(Opcode.CMSG_MOVE_SET_FLY)]
        [Parser(Opcode.CMSG_MOVE_SET_PITCH)]
        [Parser(Opcode.CMSG_MOVE_SET_RUN_MODE)]
        [Parser(Opcode.CMSG_MOVE_SET_WALK_MODE)]
        [Parser(Opcode.CMSG_MOVE_START_ASCEND)]
        [Parser(Opcode.CMSG_MOVE_START_BACKWARD)]
        [Parser(Opcode.CMSG_MOVE_START_DESCEND)]
        [Parser(Opcode.CMSG_MOVE_START_FORWARD)]
        [Parser(Opcode.CMSG_MOVE_START_PITCH_DOWN)]
        [Parser(Opcode.CMSG_MOVE_START_PITCH_UP)]
        [Parser(Opcode.CMSG_MOVE_START_SWIM)]
        [Parser(Opcode.CMSG_MOVE_START_TURN_LEFT)]
        [Parser(Opcode.CMSG_MOVE_START_TURN_RIGHT)]
        [Parser(Opcode.CMSG_MOVE_START_STRAFE_LEFT)]
        [Parser(Opcode.CMSG_MOVE_START_STRAFE_RIGHT)]
        [Parser(Opcode.CMSG_MOVE_STOP)]
        [Parser(Opcode.CMSG_MOVE_STOP_ASCEND)]
        [Parser(Opcode.CMSG_MOVE_STOP_PITCH)]
        [Parser(Opcode.CMSG_MOVE_STOP_STRAFE)]
        [Parser(Opcode.CMSG_MOVE_STOP_SWIM)]
        [Parser(Opcode.CMSG_MOVE_STOP_TURN)]
        [Parser(Opcode.SMSG_MOVE_SET_IGNORE_MOVEMENT_FORCES)]
        [Parser(Opcode.SMSG_MOVE_UPDATE_KNOCK_BACK)]
        [Parser(Opcode.SMSG_MOVE_UPDATE)]
        public static void HandlePlayerMove(Packet packet)
        {
            ReadMovementStats(packet);
        }

        [Parser(Opcode.SMSG_ON_MONSTER_MOVE)]
        public static void HandleMonsterMove(Packet packet)
        {
            packet.ReadPackedGuid128("MoverGUID");

            var pos = packet.ReadVector3("Position");
            packet.ReadUInt32("Id");
            packet.ReadVector3("Destination");

            packet.ReadEnum<SplineFlag434>("Spline Flags", TypeCode.Int32);
            packet.ReadByte("AnimTier");
            packet.ReadUInt32("TierTransStartTime");
            packet.ReadUInt32("Elapsed");
            packet.ReadUInt32("MoveTime");
            packet.ReadSingle("JumpGravity");
            packet.ReadUInt32("SpecialTime");
            var points = packet.ReadInt32("Waypoints");

            packet.ReadByte("Mode");
            packet.ReadByte("VehicleExitVoluntary");

            packet.ReadPackedGuid128("TransportGUID");
            packet.ReadByte("VehicleSeat");

            var packedDeltas = packet.ReadInt32();

            Vector3 endpos = new Vector3();
            for (int i = 0; i < points; i++)
            {
                var spot = packet.ReadVector3();

                // client always taking first point
                if (i == 0)
                    endpos = spot;

                packet.AddValue("Waypoint", spot, i);
            }

            var waypoints = new Vector3[packedDeltas];
            for (int i = 0; i < packedDeltas; i++)
            {
                var vec = packet.ReadPackedVector3();
                waypoints[i].X = vec.X;
                waypoints[i].Y = vec.Y;
                waypoints[i].Z = vec.Z;
            }

            packet.ResetBitReader();
            var type = packet.ReadBits("Face", 2);
            var monsterSplineFilter = packet.ReadBit("Has MonsterSplineFilter");

            switch (type)
            {
                case 1:
                {
                   packet.ReadVector3("FaceSpot");
                   break;
                }
                case 2:
                {
                    packet.ReadSingle("FaceDirection");
                    packet.ReadPackedGuid128("Facing GUID");
                    break;
                }
                case 3:
                {
                    packet.ReadSingle("FaceDirection");
                    break;
                }
            }

            if (monsterSplineFilter)
            {
                var count = packet.ReadUInt32("MonsterSplineFilterKey");
                packet.ReadSingle("BaseSpeed");
                packet.ReadInt16("StartOffset");
                packet.ReadSingle("DistToPrevFilterKey");

                for (int i = 0; i < count; i++)
                {
                    packet.ReadInt16("IDx", i);
                    packet.ReadInt16("Speed", i);
                }

                packet.ReadInt16("AddedToStart");

                packet.ResetBitReader();
                packet.ReadBits("FilterFlags", 2);
            }

            packet.ResetBitReader();
            packet.ReadBit("CrzTeleport");
            packet.ReadBits("Unk Bit", 2);

            // Calculate mid pos
            var mid = new Vector3
            {
                X = (pos.X + endpos.X)*0.5f,
                Y = (pos.Y + endpos.Y)*0.5f,
                Z = (pos.Z + endpos.Z)*0.5f
            };
            for (var i = 0; i < packedDeltas; ++i)
            {
                var vec = new Vector3
                {
                    X = mid.X - waypoints[i].X,
                    Y = mid.Y - waypoints[i].Y,
                    Z = mid.Z - waypoints[i].Z,
                };
                packet.AddValue("Waypoint", vec, i);
            }
        }

        [Parser(Opcode.SMSG_SET_PHASE_SHIFT_CHANGE)]
        public static void HandlePhaseShift(Packet packet)
        {
            packet.ReadPackedGuid128("Client");

            // PhaseShiftData
            packet.ReadInt32("PhaseShiftFlags");
            var count = packet.ReadInt32("PhaseShiftCount");
            packet.ReadPackedGuid128("PersonalGUID");
            for (var i = 0; i < count; ++i)
            {
                packet.ReadInt16("PhaseFlags", i);
                packet.ReadInt16("Id", i);
            }

            var preloadMapIDCount = packet.ReadInt32("PreloadMapIDsCount") / 2;
            for (var i = 0; i < preloadMapIDCount; ++i)
                packet.ReadEntry<Int16>(StoreNameType.Map, "PreloadMapID", i);

            var uiWorldMapAreaIDSwapsCount = packet.ReadInt32("UiWorldMapAreaIDSwap") / 2;
            for (var i = 0; i < uiWorldMapAreaIDSwapsCount; ++i)
                packet.ReadEntry<Int16>(StoreNameType.Map, "UiWorldMapAreaIDSwaps", i);

            var visibleMapIDsCount = packet.ReadInt32("VisibleMapIDsCount") / 2;
            for (var i = 0; i < visibleMapIDsCount; ++i)
                packet.ReadEntry<Int16>(StoreNameType.Map, "VisibleMapID", i);
        }

        [Parser(Opcode.SMSG_MOVE_SPLINE_SET_RUN_SPEED)]
        [Parser(Opcode.SMSG_MOVE_SPLINE_SET_FLIGHT_SPEED)]
        [Parser(Opcode.SMSG_MOVE_SPLINE_SET_SWIM_SPEED)]
        [Parser(Opcode.SMSG_MOVE_SPLINE_SET_RUN_BACK_SPEED)]
        [Parser(Opcode.SMSG_MOVE_SPLINE_SET_WALK_BACK_SPEED)]
        public static void HandleSplineSetSpeed(Packet packet)
        {
            packet.ReadPackedGuid128("Guid");
            packet.ReadSingle("Speed");
        }

        [Parser(Opcode.SMSG_FLIGHT_SPLINE_SYNC)]
        public static void HandleFlightSplineSync(Packet packet)
        {
            packet.ReadPackedGuid128("Guid");
            packet.ReadSingle("SplineDist");
        }

        [Parser(Opcode.SMSG_SET_VIGNETTE)]
        public static void HandleUnknown177(Packet packet)
        {
            packet.ReadBit("ForceUpdate");

            // VignetteInstanceIDList
            var int1 = packet.ReadInt32("RemovedCount");
            for (var i = 0; i < int1; ++i)
                packet.ReadPackedGuid128("IDs", i);

            // Added
            var int2 = packet.ReadInt32("AddedCount");
            for (var i = 0; i < int2; ++i)
                packet.ReadPackedGuid128("IDs", i);

            // VignetteClientData
            var int3 = packet.ReadInt32("VignetteClientDataCount");
            for (var i = 0; i < int3; ++i)
            {
                packet.ReadVector3("Position", i);
                packet.ReadPackedGuid128("ObjGUID", i);
                packet.ReadInt32("VignetteID", i);
                packet.ReadInt32("Unk", i);
            }

            // Updated
            var int4 = packet.ReadInt32("UpdatedCount");
            for (var i = 0; i < int4; ++i)
                packet.ReadPackedGuid128("IDs", i);

            // VignetteClientData
            var int5 = packet.ReadInt32("VignetteClientDataCount");
            for (var i = 0; i < int5; ++i)
            {
                packet.ReadVector3("Position", i);
                packet.ReadPackedGuid128("ObjGUID", i);
                packet.ReadInt32("VignetteID", i);
                packet.ReadInt32("Unk", i);
            }
        }

        [Parser(Opcode.SMSG_TRANSFER_PENDING)]
        public static void HandleTransferPending(Packet packet)
        {
            packet.ReadEntry<Int32>(StoreNameType.Map, "MapID");

            packet.ResetBitReader();

            var hasShipTransferPending = packet.ReadBit();
            var hasTransferSpell = packet.ReadBit();

            if (hasShipTransferPending)
            {
                packet.ReadEntry<UInt32>(StoreNameType.GameObject, "ID");
                packet.ReadEntry<Int32>(StoreNameType.Map, "OriginMapID");
            }

            if (hasTransferSpell)
                packet.ReadEntry<UInt32>(StoreNameType.Spell, "TransferSpellID");
        }

        [Parser(Opcode.SMSG_TRANSFER_ABORTED)]
        public static void HandleTransferAborted(Packet packet)
        {
            packet.ReadEntry<Int32>(StoreNameType.Map, "MapID");
            packet.ReadByte("Arg");
            packet.ReadEnum<TransferAbortReason>("TransfertAbort", 5);
        }

        [Parser(Opcode.SMSG_ABORT_NEW_WORLD)]
        public static void HandleAbortNewWorld(Packet packet)
        {
        }

        [Parser(Opcode.SMSG_MOVE_UPDATE_TELEPORT)]
        public static void HandleMoveUpdateTeleport(Packet packet)
        {
            ReadMovementStats(packet);

            var int32 = packet.ReadInt32("MovementForcesCount");
            for (int i = 0; i < int32; i++)
            {
                packet.ReadPackedGuid128("ID", i);
                packet.ReadVector3("Direction", i);
                packet.ReadInt32("TransportID", i);
                packet.ReadSingle("Magnitude", i);
                packet.ReadBits("Type", 2, i);
            }

            packet.ResetBitReader();

            var bit260 = packet.ReadBit("HasWalkSpeed");
            var bit276 = packet.ReadBit("HasRunSpeed");
            var bit52 = packet.ReadBit("HasRunBackSpeed");
            var bit28 = packet.ReadBit("HasSwimSpeed");
            var bit76 = packet.ReadBit("HasSwimBackSpeed");
            var bit20 = packet.ReadBit("HasFlightSpeed");
            var bit268 = packet.ReadBit("HasFlightBackSpeed");
            var bit60 = packet.ReadBit("HasTurnRate");
            var bit68 = packet.ReadBit("HasPitchRate");

            if (bit260)
                packet.ReadSingle("WalkSpeed");

            if (bit276)
                packet.ReadSingle("RunSpeed");

            if (bit52)
                packet.ReadSingle("RunBackSpeed");

            if (bit28)
                packet.ReadSingle("SwimSpeed");

            if (bit76)
                packet.ReadSingle("SwimBackSpeed");

            if (bit20)
                packet.ReadSingle("FlightSpeed");

            if (bit268)
                packet.ReadSingle("FlightBackSpeed");

            if (bit60)
                packet.ReadSingle("TurnRate");

            if (bit68)
                packet.ReadSingle("PitchRate");
        }

        [Parser(Opcode.SMSG_MOVE_ENABLE_GRAVITY)]
        [Parser(Opcode.SMSG_MOVE_DISABLE_GRAVITY)]
        [Parser(Opcode.SMSG_MOVE_LAND_WALK)]
        [Parser(Opcode.SMSG_MOVE_ROOT)]
        [Parser(Opcode.SMSG_MOVE_SET_CAN_FLY)]
        [Parser(Opcode.SMSG_MOVE_ENABLE_CAN_TRANSITION_BETWEEN_SWIM_AND_FLY)]
        [Parser(Opcode.SMSG_MOVE_SET_HOVER)]
        [Parser(Opcode.SMSG_MOVE_UNSET_CAN_FLY)]
        [Parser(Opcode.SMSG_MOVE_DISABLE_CAN_TRANSITION_BETWEEN_SWIM_AND_FLY)]
        [Parser(Opcode.SMSG_MOVE_UNSET_HOVER)]
        [Parser(Opcode.SMSG_MOVE_UNROOT)]
        [Parser(Opcode.SMSG_MOVE_WATER_WALK)]
        [Parser(Opcode.SMSG_MOVE_SET_FEATHER_FALL)]
        public static void HandleMovementIndex(Packet packet)
        {
            packet.ReadPackedGuid128("MoverGUID");
            packet.ReadInt32("SequenceIndex");
        }

        [Parser(Opcode.SMSG_FORCE_RUN_SPEED_CHANGE)]
        [Parser(Opcode.SMSG_FORCE_SWIM_SPEED_CHANGE)]
        [Parser(Opcode.SMSG_MOVE_SET_FLIGHT_SPEED)]
        [Parser(Opcode.SMSG_MOVE_SET_WALK_SPEED)]
        [Parser(Opcode.SMSG_MOVE_SET_RUN_BACK_SPEED)]
        public static void HandleMovementIndexSpeed(Packet packet)
        {
            packet.ReadPackedGuid128("MoverGUID");
            packet.ReadInt32("SequenceIndex");

            packet.ReadSingle("Speed");
        }

        [Parser(Opcode.SMSG_MOVE_SET_VEHICLE_REC_ID)]
        public static void HandleMoveSetVehicleRecID(Packet packet)
        {
            packet.ReadPackedGuid128("MoverGUID");
            packet.ReadInt32("SequenceIndex");

            packet.ReadInt32("VehicleRecID");
        }

        [Parser(Opcode.SMSG_MOVE_SET_COLLISION_HEIGHT)]
        public static void HandleSetCollisionHeight(Packet packet)
        {
            packet.ReadPackedGuid128("MoverGUID");
            packet.ReadInt32("SequenceIndex");
            packet.ReadSingle("Scale");
            packet.ReadSingle("Height");
            packet.ReadInt32("MountDisplayID");

            packet.ResetBitReader();

            packet.ReadBits("Reason", 2);
        }

        [Parser(Opcode.SMSG_MOVE_TELEPORT)]
        public static void HandleMoveTeleport(Packet packet)
        {
            packet.ReadPackedGuid128("MoverGUID");
            packet.ReadInt32("SequenceIndex");
            packet.ReadVector3("Position");
            packet.ReadSingle("Facing");

            var bit72 = packet.ReadBit("HasTransport");
            var bit56 = packet.ReadBit("HasVehicleTeleport");

            if (bit72)
                packet.ReadPackedGuid128("TransportGUID");

            // VehicleTeleport
            if (bit56)
            {
                packet.ReadByte("VehicleSeatIndex");
                packet.ReadBit("VehicleExitVoluntary");
                packet.ReadBit("VehicleExitTeleport");
            }
        }

        [Parser(Opcode.CMSG_MOVE_GRAVITY_DISABLE_ACK)]
        [Parser(Opcode.CMSG_MOVE_GRAVITY_ENABLE_ACK)]
        [Parser(Opcode.CMSG_MOVE_HOVER_ACK)]
        [Parser(Opcode.CMSG_MOVE_KNOCK_BACK_ACK)]
        [Parser(Opcode.CMSG_MOVE_WATER_WALK_ACK)]
        [Parser(Opcode.CMSG_MOVE_FORCE_ROOT_ACK)]
        [Parser(Opcode.CMSG_MOVE_FORCE_UNROOT_ACK)]
        [Parser(Opcode.CMSG_MOVE_SET_CAN_FLY_ACK)]
        [Parser(Opcode.CMSG_MOVE_ENABLE_SWIM_TO_FLY_TRANS_ACK)]
        public static void HandleMovementAck(Packet packet)
        {
            ReadMovementAck(ref packet);
        }

        [Parser(Opcode.CMSG_MOVE_FORCE_FLIGHT_SPEED_CHANGE_ACK)]
        [Parser(Opcode.CMSG_MOVE_FORCE_RUN_SPEED_CHANGE_ACK)]
        [Parser(Opcode.CMSG_MOVE_FORCE_RUN_BACK_SPEED_CHANGE_ACK)]
        [Parser(Opcode.CMSG_MOVE_FORCE_SWIM_SPEED_CHANGE_ACK)]
        [Parser(Opcode.CMSG_MOVE_FORCE_WALK_SPEED_CHANGE_ACK)]
        public static void HandleMovementSpeedAck(Packet packet)
        {
            ReadMovementAck(ref packet);
            packet.ReadSingle("Speed");
        }

        [Parser(Opcode.CMSG_MOVE_REMOVE_MOVEMENT_FORCE_ACK)]
        public static void HandleMoveRemoveMovementForceAck(Packet packet)
        {
            ReadMovementAck(ref packet);
            packet.ReadPackedGuid128("TriggerGUID");
        }

        [Parser(Opcode.SMSG_MOVE_REMOVE_MOVEMENT_FORCE)]
        public static void HandleMoveRemoveMovementForce(Packet packet)
        {
            packet.ReadPackedGuid128("MoverGUID");
            packet.ReadInt32("SequenceIndex");
            packet.ReadPackedGuid128("TriggerGUID");
        }

        [Parser(Opcode.SMSG_MOVE_UPDATE_REMOVE_MOVEMENT_FORCE)]
        public static void HandleMoveUpdateRemoveMovementForce(Packet packet)
        {
            ReadMovementStats(packet);
            packet.ReadPackedGuid128("TriggerGUID");
        }

        [Parser(Opcode.CMSG_MOVE_APPLY_MOVEMENT_FORCE_ACK)]
        public static void HandleMoveApplyMovementForceAck(Packet packet)
        {
            ReadMovementAck(ref packet);
            packet.ReadPackedGuid128("TriggerGUID");
            packet.ReadVector3("Direction");
            packet.ReadInt32("TransportID");
            packet.ReadSingle("Facing");

            packet.ResetBitReader();

            packet.ReadBits("Reason", 2);
        }

        [Parser(Opcode.SMSG_MOVE_APPLY_MOVEMENT_FORCE)]
        public static void HandleMoveApplyMovementForce(Packet packet)
        {
            packet.ReadPackedGuid128("MoverGUID");
            packet.ReadInt32("SequenceIndex");

            packet.ReadPackedGuid128("TriggerGUID");
            packet.ReadVector3("Direction");
            packet.ReadInt32("TransportID");
            packet.ReadSingle("Facing");

            packet.ResetBitReader();

            packet.ReadBits("Reason", 2);
        }

        [Parser(Opcode.SMSG_MOVE_UPDATE_APPLY_MOVEMENT_FORCE)]
        public static void HandleMoveUpdateApplyMovementForce(Packet packet)
        {
            ReadMovementStats(packet);
            ReadMovementForce(packet);
        }

        [Parser(Opcode.CMSG_MOVE_SET_COLLISION_HEIGHT_ACK)]
        public static void HandleMoveSetCollisionHeightAck(Packet packet)
        {
            ReadMovementAck(ref packet);
            packet.ReadSingle("Height");
            packet.ReadInt32("MountDisplayID");

            packet.ResetBitReader();

            packet.ReadBits("Reason", 2);
        }

        [Parser(Opcode.CMSG_MOVE_TELEPORT_ACK)]
        public static void HandleMoveTeleportAck(Packet packet)
        {
            packet.ReadPackedGuid128("MoverGUID");
            packet.ReadInt32("AckIndex");
            packet.ReadInt32("MoveTime");
        }

        [Parser(Opcode.SMSG_MOVE_SET_ACTIVE_MOVER)]
        [Parser(Opcode.SMSG_MOVE_SPLINE_ROOT)]
        [Parser(Opcode.SMSG_MOVE_SPLINE_UNROOT)]
        [Parser(Opcode.SMSG_MOVE_SPLINE_DISABLE_GRAVITY)]
        [Parser(Opcode.SMSG_MOVE_SPLINE_ENABLE_GRAVITY)]
        [Parser(Opcode.SMSG_MOVE_SPLINE_DISABLE_COLLISION)]
        [Parser(Opcode.SMSG_MOVE_SPLINE_ENABLE_COLLISION)]
        [Parser(Opcode.SMSG_MOVE_SPLINE_SET_FEATHER_FALL)]
        [Parser(Opcode.SMSG_MOVE_SPLINE_SET_NORMAL_FALL)]
        [Parser(Opcode.SMSG_MOVE_SPLINE_SET_HOVER)]
        [Parser(Opcode.SMSG_MOVE_SPLINE_UNSET_HOVER)]
        [Parser(Opcode.SMSG_MOVE_SPLINE_SET_WATER_WALK)]
        [Parser(Opcode.SMSG_MOVE_SPLINE_START_SWIM)]
        [Parser(Opcode.SMSG_MOVE_SPLINE_STOP_SWIM)]
        [Parser(Opcode.SMSG_MOVE_SPLINE_SET_RUN_MODE)]
        [Parser(Opcode.SMSG_MOVE_SPLINE_SET_WALK_MODE)]
        [Parser(Opcode.SMSG_MOVE_SPLINE_SET_FLYING)]
        [Parser(Opcode.SMSG_MOVE_SPLINE_UNSET_FLYING)]
        [Parser(Opcode.SMSG_MOVE_SPLINE_SET_LAND_WALK)]
        public static void HandleSplineMoveGravityDisable(Packet packet)
        {
            packet.ReadPackedGuid128("MoverGUID");
        }

        [Parser(Opcode.SMSG_SET_PLAY_HOVER_ANIM)]
        public static void HandlePlayHoverAnim(Packet packet)
        {
            packet.ReadPackedGuid128("UnitGUID");
            packet.ReadBit("PlayHoverAnim");
        }

        [Parser(Opcode.SMSG_MOVE_UPDATE_WALK_SPEED)]
        [Parser(Opcode.SMSG_MOVE_UPDATE_RUN_SPEED)]
        [Parser(Opcode.SMSG_MOVE_UPDATE_RUN_BACK_SPEED)]
        [Parser(Opcode.SMSG_MOVE_UPDATE_SWIM_SPEED)]
        [Parser(Opcode.SMSG_MOVE_UPDATE_SWIM_BACK_SPEED)]
        [Parser(Opcode.SMSG_MOVE_UPDATE_TURN_RATE)]
        [Parser(Opcode.SMSG_MOVE_UPDATE_FLIGHT_SPEED)]
        [Parser(Opcode.SMSG_MOVE_UPDATE_FLIGHT_BACK_SPEED)]
        [Parser(Opcode.SMSG_MOVE_UPDATE_PITCH_RATE)]
        public static void HandleMovementUpdateSpeed(Packet packet)
        {
            ReadMovementStats(packet);
            packet.ReadSingle("Speed");
        }

        [Parser(Opcode.SMSG_ADJUST_SPLINE_DURATION)]
        public static void HandleAdjustSplineDuration(Packet packet)
        {
            packet.ReadPackedGuid128("Unit");
            packet.ReadSingle("Scale");
        }

        [Parser(Opcode.SMSG_CLIENT_CONTROL_UPDATE)]
        public static void HandleClientControlUpdate(Packet packet)
        {
            packet.ReadPackedGuid128("Unit");
            packet.ReadBit("On");
        }

        [Parser(Opcode.CMSG_MOVE_SPLINE_DONE)]
        public static void HandleMoveSplineDone(Packet packet)
        {
            ReadMovementStats(packet);
            packet.ReadInt32("SplineID");
        }

        [Parser(Opcode.CMSG_SET_ACTIVE_MOVER)]
        public static void HandleSetActiveMover(Packet packet)
        {
            packet.ReadPackedGuid128("ActiveMover");
        }

        [Parser(Opcode.SMSG_MOVE_UPDATE_COLLISION_HEIGHT)]
        public static void HandleMoveUpdateCollisionHeight434(Packet packet)
        {
            ReadMovementStats(packet);
            packet.ReadSingle("Height");
            packet.ReadSingle("Scale");
        }

        [Parser(Opcode.SMSG_SET_VEHILCE_REC_ID)]
        public static void HandleSetVehicleRecID(Packet packet)
        {
            packet.ReadPackedGuid128("VehicleGUID");
            packet.ReadInt32("VehicleRecID");
        }

        [Parser(Opcode.SMSG_SPECIAL_MOUNT_ANIM)]
        public static void HandleSpecialMountAnim(Packet packet)
        {
            packet.ReadPackedGuid128("UnitGUID");
        }

        [Parser(Opcode.SMSG_MOVE_KNOCK_BACK)]
        public static void HandleMoveKnockBack(Packet packet)
        {
            packet.ReadPackedGuid128("MoverGUID");
            packet.ReadInt32("SequenceIndex");
            packet.ReadVector2("Direction");
            packet.ReadSingle("HorzSpeed");
            packet.ReadSingle("VertSpeed");
        }

        [Parser(Opcode.CMSG_DISCARDED_TIME_SYNC_ACKS)]
        public static void HandleDiscardedTimeSyncAcks(Packet packet)
        {
            packet.ReadUInt32("MaxSequenceIndex");
        }

        [Parser(Opcode.SMSG_PLAYER_SKINNED)]
        public static void HandlePlayerSkinned(Packet packet)
        {
            packet.ReadBit("FreeRepop");
        }

        [Parser(Opcode.SMSG_MOVE_SET_COMPOUND_STATE)]
        public static void HandleMoveSetCompoundState(Packet packet)
        {
            packet.ReadPackedGuid128("MoverGUID");

            var moveStateChangeCount = packet.ReadInt32("MoveStateChangeCount");
            for (int i = 0; i < moveStateChangeCount; i++)
            {
                packet.ReadInt16("MessageID");
                packet.ReadInt32("SequenceIndex");

                packet.ResetBitReader();

                var bit12 = packet.ReadBit("HasSpeed");
                var bit32 = packet.ReadBit("HasKnockBack");
                var bit40 = packet.ReadBit("HasVehicle");
                var bit56 = packet.ReadBit("HasCollisionHeight");
                var bit104 = packet.ReadBit("HasMovementForce");
                var bit128 = packet.ReadBit("HasMoverGUID");

                if (bit12)
                    packet.ReadSingle("Speed");

                if (bit32)
                {
                    packet.ReadSingle("HorzSpeed");
                    packet.ReadVector2("InitVertSpeed");
                    packet.ReadSingle("InitVertSpeed");
                }

                if (bit40)
                    packet.ReadInt32("VehicleRecID");

                if (bit56)
                {
                    packet.ReadSingle("Height");
                    packet.ReadSingle("Scale");
                    packet.ReadBits("UpdateCollisionHeightReason", 2);
                }

                if (bit104)
                    ReadMovementForce(packet);

                if (bit128)
                    packet.ReadPackedGuid128("MoverGUID");
            }
        }
    }
}
