using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PayPal.Api;

namespace RailwaySystem.HelperClasses
{
    public class PayPalPaymentBuilder
    {
        private APIContext apiContext;
        private Payment payment;
        private ItemList itemList;
        private Payer payer;
        private Amount amount;
        private List<Transaction> transactions;
        private RedirectUrls redirectUrl;
        RailwaySystem.Entities.Ticket ticket;

        public PayPalPaymentBuilder(APIContext apiContext, string redirectUrl)
        {
            this.apiContext = apiContext;

            this.redirectUrl = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };

            payer = new Payer() { payment_method = "paypal" };
            itemList = new ItemList();
            transactions = new List<Transaction>();
        }

        public PayPalPaymentBuilder AddItem(RailwaySystem.Entities.Ticket ticket)
        {
            itemList.items = new List<Item>();
            for(int i = 0; i < ticket.Quantity; i++)
            {
                itemList.items.Add(new Item()
                {
                    name = "Railway System E-Ticket",
                    currency = "USD",
                    price = "" + ticket.Price,
                    quantity = "1",
                    /*sku = "sku"*/
                });
            }
            //this.item = new Item()
            //{
            //    name = "Railway System E-Ticket",
            //    currency = "USD",
            //    price = "" + ticket.Price,
            //    quantity = "1",
            //    /*sku = "sku"*/
            //};
            //itemList.items.Add(this.item);
            this.ticket = ticket;
            return this;
        }

        public Payment CreatePayment()
        {
            Details details = new Details()
            {
                subtotal = "" + ticket.Price * ticket.Quantity,
                tax = "0",
                shipping = "0"
            };


            amount = new Amount()
            {
                currency = itemList.items[0].currency,
                total = details.subtotal,
                details = details
            };

            Random random = new Random();
            transactions.Add(new Transaction()
            {
                description = "Railway System E-Ticket Service\n"
                                + ticket.Quantity + " ticket" + (ticket.Quantity > 1 ? "s\n" : "\n")
                                + "Route: " + ticket.BeginStation + " - " + ticket.EndStation + "\n"
                                + "Date of departure: " + ticket.Departure + "\n",
                amount = this.amount,
                invoice_number = "RND_INVOICE_NUM_" + random.Next(1000000),
                item_list = itemList
            });

            payment = new Payment()
            {
                intent = "sale",
                payer = this.payer,
                transactions = this.transactions,
                redirect_urls = this.redirectUrl
            };

            return payment.Create(this.apiContext);
        }

        public static Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            Payment payment = new Payment()
            {
                id = paymentId
            };
            return payment.Execute(apiContext, paymentExecution);
        }

    }
}