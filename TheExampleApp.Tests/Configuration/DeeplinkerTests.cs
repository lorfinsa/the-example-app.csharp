﻿using Contentful.Core.Configuration;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TheExampleApp.Configuration;
using Xunit;

namespace TheExampleApp.Tests.Configuration
{
    public class DeeplinkerTests
    {

        [Fact]
        public async Task DeeplinkerShouldAddContentfulOptionsFromQuery()
        {
            //Arrange
            var optionsManager = new Mock<IContentfulOptionsManager>();
            optionsManager.SetupGet(c => c.Options).Returns(new ContentfulOptions { DeliveryApiKey = "Fasanhöns" });
            var deeplinker = new Deeplinker((innerHttpContext) =>
            {
                return Task.CompletedTask;
            }, optionsManager.Object);
            var mockSession = new Mock<ISession>();
            mockSession.Setup(c => c.Set(nameof(ContentfulOptions), It.IsAny<byte[]>())).Verifiable();

            var mockQuery = new Mock<IQueryCollection>();
            mockQuery.Setup(c => c.ContainsKey(It.Is<string>(s => s == "space_id" || s == "preview_token" || s == "delivery_token"))).Returns(true);
            mockQuery.SetupGet(c => c["delivery_token"]).Returns("McKinley");
            mockQuery.SetupGet(c => c["space_id"]).Returns("Kilimanjaro");
            mockQuery.SetupGet(c => c["preview_token"]).Returns("Fuji");

            var mockRequest = new Mock<HttpRequest>();
            mockRequest.SetupGet(r => r.Query).Returns(mockQuery.Object);

            var mockContext = new Mock<HttpContext>();
            mockContext.Setup(c => c.Session).Returns(mockSession.Object);
            mockContext.SetupGet(c => c.Request).Returns(mockRequest.Object);
            
            //Act
            await deeplinker.Invoke(mockContext.Object);

            //Assert
            mockSession.Verify(c => c.Set(nameof(ContentfulOptions), It.Is<byte[]>(b => Encoding.UTF8.GetString(b).Contains("McKinley"))));
        }

        [Fact]
        public async Task DeeplinkerShouldNotAddContentfulOptionsFromQueryWithMissingValues()
        {
            //Arrange
            var optionsManager = new Mock<IContentfulOptionsManager>();
            optionsManager.SetupGet(c => c.Options).Returns(new ContentfulOptions { DeliveryApiKey = "Fasanhöns" });
            var deeplinker = new Deeplinker((innerHttpContext) =>
            {
                return Task.CompletedTask;
            }, optionsManager.Object);
            var mockSession = new Mock<ISession>();
            mockSession.Setup(c => c.Set(nameof(ContentfulOptions), It.IsAny<byte[]>())).Verifiable();

            var mockQuery = new Mock<IQueryCollection>();
            mockQuery.Setup(c => c.ContainsKey(It.Is<string>(s => s == "space_id" || s == "preview_token"))).Returns(true);
            mockQuery.SetupGet(c => c["delivery_token"]).Returns("McKinley");
            mockQuery.SetupGet(c => c["space_id"]).Returns("Kilimanjaro");
            mockQuery.SetupGet(c => c["preview_token"]).Returns("Fuji");

            var mockRequest = new Mock<HttpRequest>();
            mockRequest.SetupGet(r => r.Query).Returns(mockQuery.Object);

            var mockContext = new Mock<HttpContext>();
            mockContext.Setup(c => c.Session).Returns(mockSession.Object);
            mockContext.SetupGet(c => c.Request).Returns(mockRequest.Object);

            //Act
            await deeplinker.Invoke(mockContext.Object);

            //Assert
            mockSession.Verify(c => c.Set(nameof(ContentfulOptions), It.Is<byte[]>(b => Encoding.UTF8.GetString(b).Contains("McKinley"))),Times.Never);
        }

        [Fact]
        public async Task DeeplinkerShouldAddEditorialFeaturesFromQuery()
        {
            //Arrange
            var optionsManager = new Mock<IContentfulOptionsManager>();
            optionsManager.SetupGet(c => c.Options).Returns(new ContentfulOptions { DeliveryApiKey = "Fasanhöns" });
            var deeplinker = new Deeplinker((innerHttpContext) =>
            {
                return Task.CompletedTask;
            }, optionsManager.Object);
            var mockSession = new Mock<ISession>();
            mockSession.Setup(c => c.Set("EditorialFeatures", It.IsAny<byte[]>())).Verifiable();

            var mockQuery = new Mock<IQueryCollection>();
            mockQuery.Setup(c => c.ContainsKey(It.Is<string>(s => s == "enable_editorial_features"))).Returns(true);

            var mockRequest = new Mock<HttpRequest>();
            mockRequest.SetupGet(r => r.Query).Returns(mockQuery.Object);

            var mockContext = new Mock<HttpContext>();
            mockContext.Setup(c => c.Session).Returns(mockSession.Object);
            mockContext.SetupGet(c => c.Request).Returns(mockRequest.Object);

            //Act
            await deeplinker.Invoke(mockContext.Object);

            //Assert
            mockSession.Verify(c => c.Set("EditorialFeatures", It.Is<byte[]>(b => Encoding.UTF8.GetString(b).Contains("Enabled"))));
        }
    }
}
