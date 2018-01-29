﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CarryOnWebApi;
using CarryOnWebApi.Controllers;
using Services;
using DAL;
using Services.Interfaces;
using Entities;
using DAL.Helper;
using DAL.Mapper;

namespace CarryOnWebApi.Tests.Controllers
{
    [TestClass]
    public class TransportAvControllerTest
    {
        #region Private Variable

        IReqGoodTransferService reqGoodTransferService;
        ITransportAvService transportAvService;
        IDalManager dbManager;
        private ILogService logger;
        private IConfigurationProvider configuration;
        IAccountService accountService;

        #endregion

        #region Test Initialize

        [TestInitialize]
        public void TestInitialize()
        {
            dbManager = new DalManager();
            this.configuration = new Configuration();
            this.logger = new Log4NetLogService(configuration);
            reqGoodTransferService = new ReqGoodTransferService(dbManager, logger);
            transportAvService = new TransportAvService(dbManager, logger);
            accountService = new AccountService(logger, dbManager);
        }

        #endregion

        #region Utility Function

        private UserModel AddUser_ForTest()
        {
            AccountController accountController = new AccountController(accountService, logger, configuration);
            var userToAdd = MockUserHelper.getUser_feModel();
            userToAdd.UserName = userToAdd.UserEmail;
            var userAddResult = accountController.CreateUser(userToAdd);
            /* Check if the user is already existing */
            if (userAddResult.OperationResult == false && userAddResult.ResultMessage == Entities.enums.ErrorsEnum.USERNAME_ALREADY_PRESENT)
            {
                userToAdd = accountService.GetUserByEmail(userToAdd.UserEmail);
            }
            Assert.IsNotNull(userAddResult);
            return userToAdd;
        }

        private TransportAvModel AddTrAv_ForTest(UserModel userToAdd)
        {
            // Arrange
            TransportAvController trAvController = new TransportAvController(transportAvService, logger, configuration);

            var trAvToAddDb = MockHelper.getTransportAvList().FirstOrDefault();
            trAvToAddDb.UserId = userToAdd.UserId;
            trAvToAddDb.UserName = userToAdd.UserName;
            trAvToAddDb.UserEmail = userToAdd.UserEmail;
            if (trAvToAddDb == null)
            {
                return null;
            }
            var trAvToAddModel = TransportAvMapper.TransportAv_DbToModel(trAvToAddDb);
            trAvToAddModel.ReqGoodTransportOpt = MockHelper.getTransportOpt();
            var addResult = trAvController.PublishItem(trAvToAddModel);
            // Assert
            Assert.IsNotNull(addResult);
            Assert.IsTrue(addResult.OperationResult);

            return trAvToAddModel;
        }

        #endregion

        #region Get Filtered and not filtered list

        [TestMethod]
        public void GetAllNoFilters()
        {
            // Arrange
            TransportAvController trAvController = new TransportAvController(transportAvService, logger, configuration);

            // filter params
            var filterParams = new SearchRtFilter();

            // Act
            var result = trAvController.FilteredTrAv(filterParams);

            // Assert
            Assert.IsNotNull(result);
        }

        #endregion

        #region Add-Create User

        [TestMethod]
        public void AddRqgt()
        {
            // Arrange
            TransportAvController trAvController = new TransportAvController(transportAvService, logger, configuration);

            // First Add user if not existing
            var userToAdd = AddUser_ForTest();

            var trAvToAddDb = MockHelper.getTransportAvList().FirstOrDefault();
            trAvToAddDb.UserId = userToAdd.UserId;
            trAvToAddDb.UserName = userToAdd.UserName;
            trAvToAddDb.UserEmail = userToAdd.UserEmail;
            Assert.IsNotNull(trAvToAddDb);

            var trAvToAddModel = TransportAvMapper.TransportAv_DbToModel(trAvToAddDb);
            trAvToAddModel.ReqGoodTransportOpt = MockHelper.getTransportOpt();
            var addResult = trAvController.PublishItem(trAvToAddModel);
            // Assert
            Assert.IsNotNull(addResult);
            Assert.IsTrue(addResult.OperationResult);
        }

        #endregion

        #region Get Rqgt

        [TestMethod]
        public void GetRqgtDetails()
        {
            // Arrange
            TransportAvController trAvController = new TransportAvController(transportAvService, logger, configuration);

            // First Add user if not existing
            var userToAdd = AddUser_ForTest();

            // First Insert Rqgt
            var trAvToAddModel = AddTrAv_ForTest(userToAdd);

            // Get Rqgt Details
            var detailsResult = trAvController.GetTrAvDetails(trAvToAddModel.Id);

            // Assert
            Assert.IsNotNull(detailsResult);
            Assert.IsTrue(detailsResult.OperationResult);
        }

        #endregion

        #region Delete Rqgt

        [TestMethod]
        public void DeleteRqgt()
        {
            // Arrange
            TransportAvController trAvController = new TransportAvController(transportAvService, logger, configuration);

            // First Add user if not existing
            var userToAdd = AddUser_ForTest();

            // First Insert Rqgt
            var trAvToAddModel = AddTrAv_ForTest(userToAdd);

            // Delete Rqgt
            var detailsResult = trAvController.Delete(trAvToAddModel.Id);

            // Assert
            Assert.IsNotNull(detailsResult);
            Assert.IsTrue(detailsResult.OperationResult);
        }

        #endregion

        #region Update Rqgt

        [TestMethod]
        public void UpdateRqgt()
        {
            // Arrange
            TransportAvController trAvController = new TransportAvController(transportAvService, logger, configuration);

            // First Add user if not existing
            var userToAdd = AddUser_ForTest();

            // First Insert Rqgt
            var trAvToAddModel = AddTrAv_ForTest(userToAdd);

            // Get Rqgt Details
            var detailsResult = trAvController.GetTrAvDetails(trAvToAddModel.Id);
            // Assert
            Assert.IsNotNull(detailsResult);
            Assert.IsTrue(detailsResult.OperationResult);

            // Update Rqgt
            var trAvToUpdateModel = detailsResult.ResultData;
            trAvToUpdateModel.DateTransportFixed = DateTime.Today.AddYears(1);
            trAvToUpdateModel.DateTransportInfo = "UPDATED";
            trAvToUpdateModel.UserLang = "UPDATED";
            trAvToUpdateModel.UserTEL2 = "UPDATED";
            trAvToUpdateModel.UserTELE = "UPDATED";
            trAvToUpdateModel.UserEmail = "UPDATED";
            trAvToUpdateModel.UserLang = "UPDATED";
            trAvToUpdateModel.fromAddress.formatted_address = "UPDATED";
            trAvToAddModel.destAddress.formatted_address = "UPDATED";
            var updatedResult = trAvController.Put(trAvToUpdateModel);

            // Assert
            Assert.IsNotNull(updatedResult);
            Assert.IsTrue(updatedResult.OperationResult);
        }

        #endregion
    }
}
