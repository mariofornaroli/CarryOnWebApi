﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using DAL;
using Services.Interfaces;
using Entities.enums;
using DAL.Mapper;

namespace Services
{
    public class TransportAvService : ITransportAvService
    {
        public IDalManager _dbManager;
        private ILogService logger;

        public TransportAvService(IDalManager dbManager, ILogService logger)
        {
            _dbManager = dbManager;
            this.logger = logger;
        }

        public List<TransportAvModel> GetTransportAv(Guid? reqId, Guid? userId)
        {
            logger.Log(() => GetTransportAv(reqId, userId));
            var retList = new List<TransportAvModel>();

            /* Get items from db */
            var db_TransportAv = _dbManager.GetTransportAv_ByKeySomeEqualFields(reqId, null, null, null, 
                null, null, null, userId, null);

            /* Convert them to model */
            foreach (var dbItem in db_TransportAv)
            {
                retList.Add(TransportAvMapper.TransportAv_DbToModel(dbItem));
            }

            ///* Foreach item, get transport options */
            //foreach (var retItem in retList)
            //{
            //    retItem.ReqGoodTransportOpt = new List<ReqGoodTransportOptions>();
            //    var transportGoodOpt = _dbManager.GetReqGoodTransportOptionsByTransportId(retItem.Id);
            //    retItem.ReqGoodTransportOpt.AddRange(transportGoodOpt.Select(x => TransportAvMapper.TransportAvOption_DbToModel(x)).ToList());
            //}

            return retList;
        }

        public TransportAvModel GetTrAvDetails(Guid? reqId)
        {
            logger.Log(() => GetTrAvDetails(reqId));

            /* Get items from db */
            var db_ReqGoodTransfer = _dbManager.GetTransportAv_ByKeyFields(reqId).FirstOrDefault();

            if (db_ReqGoodTransfer == null)
            {
                return null;
            }

            var retModel = TransportAvMapper.TransportAv_DbToModel(db_ReqGoodTransfer);

            // Get transfer options
            var options = _dbManager.GetReqGoodTransportOptionsByTransportId((Guid)reqId);
            retModel.ReqGoodTransportOpt = options.Select(x => ReqGoodTransferMapper.ReqGoodTransferOption_DbToModel(x)).ToList();

            return retModel;
        }

        public BaseResultModel InsertTransportAv(TransportAvModel rqtModel, UserModel user)
        {
            logger.Log(() => InsertTransportAv(rqtModel, user));
            var resultModel = new BaseResultModel() { OperationResult = true };

            try
            {
                //check if username exists
                var existingUser = _dbManager.GetUserByUsername(user.UserEmail);
                if (existingUser == null)
                {
                    resultModel.OperationResult = false;
                    resultModel.ResultMessage = ErrorsEnum.USER_NOT_PRESENT;
                    return resultModel;
                }

                /* Add addresses from */
                var addressFromId = Guid.NewGuid();
                var addressDestId = Guid.NewGuid();
                var reqGoodTransportId = Guid.NewGuid();

                /* Add addresses from */
                var addressFrom = GeoCodeMapper.GeoCodeAddress_ModelToDb(rqtModel.fromAddress);
                _dbManager.InsertAddress(addressFrom);

                /* Add addresses destination */
                var addressDest = GeoCodeMapper.GeoCodeAddress_ModelToDb(rqtModel.destAddress);
                _dbManager.InsertAddress(addressDest);

                /* Add TransportAv item */
                var db_TransportAvModel = TransportAvMapper.TransportAv_ModelToDb(rqtModel);
                /* Add guid */
                db_TransportAvModel.Id = reqGoodTransportId;
                db_TransportAvModel.AddressFrom = addressFromId;
                db_TransportAvModel.AddreessDest = addressDestId;
                /* Add to db */
                _dbManager.InsertTransportAv(db_TransportAvModel);

                /* Add TransportAv options items */
                if (rqtModel.ReqGoodTransportOpt != null)
                {
                    foreach (var optItem in rqtModel.ReqGoodTransportOpt)
                    {
                        optItem.TransportId = reqGoodTransportId;
                        _dbManager.InsertReqGoodTransferOption(optItem);
                    }
                }

                return resultModel;
            }
            catch (Exception e)
            {
                logger.Log(() => InsertTransportAv(rqtModel, user), e);
                resultModel.OperationResult = false;
                resultModel.ResultMessage = ErrorsEnum.GENERIC_ERROR;
            }
            return resultModel;
        }

        public BaseResultModel UpdateTransportAv(TransportAvModel rqtModel, UserModel user)
        {
            logger.Log(() => UpdateTransportAv(rqtModel, user));
            var resultModel = new BaseResultModel() { OperationResult = true };

            try
            {
                /* Check if item exists on db for that user */
                var existing_TransportAv = _dbManager.GetTransportAv_ByKeySomeEqualFields(rqtModel.Id, null, null, null
                    , null, null, null, user.UserId, null).FirstOrDefault();
                if (existing_TransportAv == null)
                {
                    resultModel.OperationResult = false;
                    resultModel.ResultMessage = ErrorsEnum.USERNAME_ALREADY_PRESENT;
                    return resultModel;
                }

                /* Add addresses from */
                var addressFrom = GeoCodeMapper.GeoCodeAddress_ModelToDb(rqtModel.fromAddress);
                _dbManager.UpdateAddress(addressFrom);

                /* Add addresses destination */
                var addressDest = GeoCodeMapper.GeoCodeAddress_ModelToDb(rqtModel.destAddress);
                _dbManager.UpdateAddress(addressDest);

                /* Add TransportAv item */
                var db_TransportAvModel = TransportAvMapper.TransportAv_ModelToDb(rqtModel);

                _dbManager.UpdateTransportAv(db_TransportAvModel);

                /* Add TransportAv options items */
                foreach (var optItem in rqtModel.ReqGoodTransportOpt)
                {
                    _dbManager.UpdateReqGoodTransferOption(optItem);
                }

                return resultModel;
            }
            catch (Exception e)
            {
                logger.Log(() => UpdateTransportAv(rqtModel, user), e);
                resultModel.OperationResult = false;
                resultModel.ResultMessage = ErrorsEnum.GENERIC_ERROR;
            }
            return resultModel;
        }

        public BaseResultModel DeleteTransportAv(Guid rgtId, UserModel user)
        {
            logger.Log(() => DeleteTransportAv(rgtId, user));
            var resultModel = new BaseResultModel() { OperationResult = true };

            try
            {
                /* Check if item exists on db for the current user*/
                var existing_TransportAv = _dbManager.GetTransportAv_ByKeySomeEqualFields(rgtId, null, null, null
                    , null, null, null, user.UserId, null).FirstOrDefault();
                if (existing_TransportAv == null)
                {
                    resultModel.OperationResult = false;
                    resultModel.ResultMessage = ErrorsEnum.USERNAME_ALREADY_PRESENT;
                    return resultModel;
                }

                /* Delete addresses from */
                _dbManager.DeleteAddress((Guid)existing_TransportAv.AddressFrom);
                /* Delete addresses dest */
                _dbManager.DeleteAddress((Guid)existing_TransportAv.AddreessDest);
                /* Delete TransportAv options items */
                _dbManager.DeleteReqGoodTransferOption(new ReqGoodTransportOptions {
                    TransportId = existing_TransportAv.Id,
                    OptKey = null,
                    OptValue = null
                });
                /* Delete TransportAv item */
                _dbManager.DeleteTransportAv(existing_TransportAv.Id);                
                return resultModel;
            }
            catch (Exception e)
            {
                logger.Log(() => DeleteTransportAv(rgtId, user), e);
                resultModel.OperationResult = false;
                resultModel.ResultMessage = ErrorsEnum.GENERIC_ERROR;
            }
            return resultModel;
        }
    }
}
