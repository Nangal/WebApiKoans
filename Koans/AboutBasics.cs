﻿using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using FSharpKoans.Core;

namespace Koans
{
    /**
     * Lesson 1: About Basics
     * 
     * Unlike many koans programs, Web API requires a bit of setup
     * in order to start learning how to use it. Each of the concepts
     * below will be inspected in further detail as you progress.
     */

    [Koan(Sort = 1)]
    public static class AboutBasics
    {
        [Koan]
        public static void NotFound()
        {
            // Send a GET message. The Web API API is asynchronous.
            // You should avoid calling .Result, but for testing,
            // this is desired. Fortunately, C# 5 helps deal with async.
            using (var response = WebApiKoans.Client.GetAsync("http://go.com/").Result)
                Helpers.AssertEquality(Helpers.__, response.StatusCode);
        }

        [Koan]
        public static void FirstRoute()
        {
            // In order to get anything other than a 404 response,
            // you first must add a route on the configuration.
            // Note how similar routing is to routing in MVC.
            // We'll explore routing further a little later.
            GlobalConfiguration.Configuration.Routes.MapHttpRoute(
                name: "Default",
                routeTemplate: "api"
            );

            using (var response = WebApiKoans.Client.GetAsync("http://go.com/api").Result)
                Helpers.AssertEquality(Helpers.__, response.StatusCode);
        }

        [Koan]
        public static void Ok()
        {
            GlobalConfiguration.Configuration.Routes.MapHttpRoute("Default", "api");

            // A route alone is insufficient to get past the 404.
            // You also need some form of handler. ASP.NET has IHttpHandler
            // and IHttpModule, but Web API uses ApiControllers and
            // HttpMessageHandlers. We will cover each in detail later,
            // but for now we will add an HttpMessageHandler for simplicity.
            GlobalConfiguration.Configuration.MessageHandlers.Add(new OkHandler());

            using (var response = WebApiKoans.Client.GetAsync("http://go.com/api").Result)
                Helpers.AssertEquality(Helpers.__, response.StatusCode);
        }

        class OkHandler : DelegatingHandler
        {
            // Message Handlers are very similar to a delegate taking
            // an HttpRequestMessage and returning a Task returning
            // an HttpResponseMessage. DelegatingHandlers are an implementation
            // that allow chaining. Once again, we'll take a look at this
            // in more detail later.
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = new HttpResponseMessage();

                // The best way to return a synchronous result is by using
                // a TaskCompletionSource. Look up more information about
                // Tasks at http://bradwilson.typepad.com/blog/2012/04/tpl-and-servers-pt1.html
                var tcs = new TaskCompletionSource<HttpResponseMessage>();
                tcs.SetResult(response);
                return tcs.Task;
            }
        }

        [Koan]
        public static void HelloWorld()
        {
            GlobalConfiguration.Configuration.Routes.MapHttpRoute("Default", "api");
            GlobalConfiguration.Configuration.MessageHandlers.Add(new HelloHandler());

            using (var response = WebApiKoans.Client.GetAsync("http://go.com/api").Result)
            {
                // In addition to getting the response message, you need
                // to also retrieve the content. Each message object has a
                // Content property and methods to read the contents.
                // We will look more into these in AboutContent.
                var body = response.Content.ReadAsStringAsync().Result;

                Helpers.AssertEquality("Hello, world!", body);
            }
        }

        class HelloHandler : DelegatingHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = new HttpResponseMessage
                {
                    // AboutContent will address the various types of HttpContent.
                    // For now, you may note that the HttpResponseMessage
                    // has a Content property that will accept such a type.
                    Content = new StringContent(Helpers.__)
                };

                var tcs = new TaskCompletionSource<HttpResponseMessage>();
                tcs.SetResult(response);
                return tcs.Task;
            }
        }
    }
}
