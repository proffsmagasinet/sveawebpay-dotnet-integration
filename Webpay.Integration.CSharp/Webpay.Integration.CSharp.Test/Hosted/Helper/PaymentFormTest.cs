﻿using NUnit.Framework;
using Webpay.Integration.CSharp.Hosted.Helper;
using Webpay.Integration.CSharp.Util.Security;
using Webpay.Integration.CSharp.Util.Testing;

namespace Webpay.Integration.CSharp.Test.Hosted.Helper
{
    [TestFixture]
    public class PaymentFormTest
    {
        private const string SecretWord = "secret";
        private const string MerchantId = "1234";

        [Test]
        public void TestSetForm()
        {
            string base64Payment = Base64Util.EncodeBase64String("0");
            string mac = HashUtil.CreateHash(base64Payment + SecretWord);
            PaymentForm form = WebpayConnection.CreateOrder()
                                               .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                               .SetCurrency(TestingTool.DefaultTestCurrency)
                                               .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                               .AddOrderRow(TestingTool.CreateMiniOrderRow())
                                               .AddCustomerDetails(TestingTool.CreateCompanyCustomer())
                                               .UsePayPageDirectBankOnly()
                                               .SetReturnUrl("http:myurl")
                                               .GetPaymentForm();
            form.SetMessageBase64(base64Payment)
                .SetMerchantId(MerchantId)
                .SetSecretWord(SecretWord)
                .SetForm();

            string expected = "<form name=\"paymentForm\" id=\"paymentForm\" method=\"post\" action=\""
                              + form.GetPayPageUrl()
                              + "\">"
                              + "<input type=\"hidden\" name=\"merchantid\" value=\""
                              + MerchantId
                              + "\" />"
                              + "<input type=\"hidden\" name=\"message\" value=\""
                              + base64Payment
                              + "\" />"
                              + "<input type=\"hidden\" name=\"mac\" value=\""
                              + mac
                              + "\" />"
                              +
                              "<noscript><p>Javascript är inaktiverat i er webbläsare, ni får dirigera om till paypage manuellt</p></noscript>"
                              + "<input type=\"submit\" name=\"submit\" value=\"Betala\" />"
                              + "</form>";

            Assert.AreEqual(expected, form.GetCompleteForm());
        }

        [Test]
        public void TestSetHtmlFields()
        {
            string base64Payment = Base64Util.EncodeBase64String("0");
            string mac = HashUtil.CreateHash(base64Payment + SecretWord);

            PaymentForm form = WebpayConnection.CreateOrder()
                                               .SetCountryCode(TestingTool.DefaultTestCountryCode)
                                               .SetClientOrderNumber(TestingTool.DefaultTestClientOrderNumber)
                                               .SetCurrency(TestingTool.DefaultTestCurrency)
                                               .AddOrderRow(TestingTool.CreateMiniOrderRow())
                                               .AddCustomerDetails(TestingTool.CreateCompanyCustomer())
                                               .UsePayPageDirectBankOnly()
                                               .SetReturnUrl("http:myurl.se")
                                               .GetPaymentForm();

            form.SetMessageBase64(base64Payment)
                .SetMerchantId(MerchantId)
                .SetSecretWord(SecretWord)
                .SetHtmlFields();

            var formHtmlFields = form.GetFormHtmlFields();
            string url = form.GetPayPageUrl();

            Assert.AreEqual("<form name=\"paymentForm\" id=\"paymentForm\" method=\"post\" action=\"" + url + "\">",
                            formHtmlFields["form_start_tag"]);
            Assert.AreEqual("<input type=\"hidden\" name=\"merchantid\" value=\"" + MerchantId + "\" />",
                            formHtmlFields["input_merchantId"]);
            Assert.AreEqual("<input type=\"hidden\" name=\"message\" value=\"" + base64Payment + "\" />",
                            formHtmlFields["input_message"]);
            Assert.AreEqual("<input type=\"hidden\" name=\"mac\" value=\"" + mac + "\" />", formHtmlFields["input_mac"]);
            Assert.AreEqual(
                "<noscript><p>Javascript är inaktiverat i er webbläsare, ni får dirigera om till paypage manuellt</p></noscript>",
                formHtmlFields["noscript_p_tag"]);
            Assert.AreEqual("<input type=\"submit\" name=\"submit\" value=\"Betala\" />", formHtmlFields["input_submit"]);
            Assert.AreEqual("</form>", formHtmlFields["form_end_tag"]);
        }
    }
}