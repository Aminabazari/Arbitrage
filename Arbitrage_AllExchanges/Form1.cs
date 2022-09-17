using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;

namespace Arbitrage_AllExchanges
{
    public partial class Form1 : Form
    {
        const int count = 5;
        const int depth = 10;
        HttpClient[] clientEx;
        HttpClient clientTelegram = new HttpClient();
        DateTime baseDate = new DateTime(1970, 1, 1);
        string[] Access_id, Secret_key, Pass;
        string owner, alarmChatId, market, HuobiPro_AccountId;
        int Findex, Lindex, HuobiPro_Precision, Kucoin_Precision;
        bool buttonFlag, sellParam;
        double Threshold, openDiff, closeDiff, amountT, maxDiff, minDiff;
        double[] high = new double[2], low = new double[2];
        bool hFlag, lFlag;
        public class Response
        {
            public object status { get; set; }
            public object code { get; set; }
            public object msg { get; set; }
        }
        public class Response_List
        {
            public string market { get; set; }
            public string bid { get; set; }
            public string ask { get; set; }
        }
        public class Response_Depth
        {
            public string[][] bids { get; set; }
            public string[][] asks { get; set; }
        }
        public class COINEX_Response_List
        {
            public COINEX_Response_List_L2 data { get; set; }
        }
        public class COINEX_Response_List_L2
        {
            public object ticker { get; set; }
        }
        public class COINEX_Response_List_L3
        {
            public string buy { get; set; }
            public string sell { get; set; }
        }
        public class COINEX_Response_Depth
        {
            public COINEX_Response_Depth_L2 data { get; set; }
        }
        public class COINEX_Response_Depth_L2
        {
            public string[][] asks { get; set; }
            public string[][] bids { get; set; }
        }
        public class COINEX_Balance
        {
            public object data { get; set; }
        }
        public class COINEX_Balance_L2
        {
            public string available { get; set; }
        }
        public class OKX_Response_List
        {
            public OKX_Response_List_L2[] data { get; set; }
        }
        public class OKX_Response_List_L2
        {
            public string instId { get; set; }
            public string askPx { get; set; }
            public string bidPx { get; set; }
        }
        public class OKX_Response_Depth
        {
            public OKX_Response_Depth_L2[] data { get; set; }
        }
        public class OKX_Response_Depth_L2
        {
            public string[][] asks { get; set; }
            public string[][] bids { get; set; }
        }
        public class OKX_Balance
        {
            public OKX_Balance_L2[] data { get; set; }
        }
        public class OKX_Balance_L2
        {
            public OKX_Balance_L3[] details { get; set; }
        }
        public class OKX_Balance_L3
        {
            public string availEq { get; set; }
        }
        public class KUCOIN_Response_List
        {
            public KUCOIN_Response_List_L2 data { get; set; }
        }
        public class KUCOIN_Response_List_L2
        {
            public KUCOIN_Response_List_L3[] ticker { get; set; }
        }
        public class KUCOIN_Response_List_L3
        {
            public string symbol { get; set; }
            public string buy { get; set; }
            public string sell { get; set; }
        }
        public class KUCOIN_Response_Depth
        {
            public KUCOIN_Response_Depth_L2 data { get; set; }
        }
        public class KUCOIN_Response_Depth_L2
        {
            public string[][] bids { get; set; }
            public string[][] asks { get; set; }
        }
        public class KUCOIN_Response_Precision
        {
            public KUCOIN_Response_Precision_L2[] data { get; set; }
        }
        public class KUCOIN_Response_Precision_L2
        {
            public string symbol { get; set; }
            public string baseIncrement { get; set; }
        }
        public class KUCOIN_Balance
        {
            public KUCOIN_Balance_L2[] data { get; set; }
        }
        public class KUCOIN_Balance_L2
        {
            public string available { get; set; }
        }
        public class GATE_Response_List_L2
        {
            public string currency_pair { get; set; }
            public string highest_bid { get; set; }
            public string lowest_ask { get; set; }
        }
        public class GATE_Response_Depth
        {
            public string[][] bids { get; set; }
            public string[][] asks { get; set; }
        }
        public class GATE_Balance
        {
            public string available { get; set; }
        }
        public class HUOBIPRO_Response_List
        {
            public HUOBIPRO_Response_List_L2[] data { get; set; }
        }
        public class HUOBIPRO_Response_List_L2
        {
            public string symbol { get; set; }
            public string ask { get; set; }
            public string bid { get; set; }
        }
        public class HUOBIPRO_Response_Depth
        {
            public HUOBIPRO_Response_Depth_L2 tick { get; set; }
        }
        public class HUOBIPRO_Response_Depth_L2
        {
            public string[][] asks { get; set; }
            public string[][] bids { get; set; }
        }
        public class HUOBIPRO_Response_Account
        {
            public HUOBIPRO_Response_Account_L2[] data { get; set; }
        }
        public class HUOBIPRO_Response_Account_L2
        {
            public string id { get; set; }
            public string type { get; set; }
        }
        public class HUOBIPRO_Response_Precision
        {
            public HUOBIPRO_Response_Precision_L2[] data { get; set; }
        }
        public class HUOBIPRO_Response_Precision_L2
        {
            public string ap { get; set; }
        }
        public class HUOBIPRO_Response_Balance
        {
            public HUOBIPRO_Response_Balance_L2 data { get; set; }
        }
        public class HUOBIPRO_Response_Balance_L2
        {
            public HUOBIPRO_Response_Balance_L3[] list { get; set; }
        }
        public class HUOBIPRO_Response_Balance_L3
        {
            public string currency { get; set; }
            public string balance { get; set; }
        }
        public Form1()
        {
            InitializeComponent();
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Initialize();
        }

        private void Initialize()
        {
            clientEx = new HttpClient[count];
            for (int i = 0; i < count; i++)
            {
                clientEx[i] = new HttpClient();
            }
            clientEx[0].BaseAddress = new Uri("https://api.coinex.com/");
            clientEx[1].BaseAddress = new Uri("https://www.okx.com");
            clientEx[2].BaseAddress = new Uri("https://api.kucoin.com");
            clientEx[3].BaseAddress = new Uri("https://api.gateio.ws");
            clientEx[4].BaseAddress = new Uri("https://api.huobi.pro");
            Access_id = new string[count];
            Secret_key = new string[count];
            Pass = new string[count];
            string readText = File.ReadAllText("Codes.txt");
            var resTemp = readText.Split('\n');
            List<string> res = new List<string>();
            for (int i = 0; i < resTemp.Length; i++)
                if (resTemp[i].Length > 0)
                    res.Add(resTemp[i].Substring(0, resTemp[i].Length - 1));
            this.Text += "(" + res[0] + ")";
            owner = res[0];
            int j = 0;
            for (int i = 0; i < count; i++)
            {
                Access_id[i] = res[++j];
                Secret_key[i] = res[++j];
                Pass[i] = res[++j];
            }
            alarmChatId = res[++j];
            buttonFlag = true;
            try
            {
                readText = File.ReadAllText("CodesForm.txt");
                resTemp = readText.Split('\n');
                Findex = int.Parse(resTemp[0].Replace("\r", ""));
                comboBox1.SelectedIndex = Findex;
                Lindex = int.Parse(resTemp[1].Replace("\r", ""));
                comboBox2.SelectedIndex = Lindex;
                market = resTemp[2].Replace("\r", "");
                textBox1.Text = resTemp[3].Replace("\r", "");
                textBox2.Text = resTemp[4].Replace("\r", "");
                textBox3.Text = resTemp[5].Replace("\r", "");
                textBox4.Text = resTemp[6].Replace("\r", "");
            }
            catch (Exception ex)
            {
                MessageBox.Show("CodesForm read Error: " + ex.ToString());
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            m_BuildGrid(dataGridView1);
            Findex = comboBox1.SelectedIndex;
            Lindex = comboBox2.SelectedIndex;

            var FRlist = await Deserialize_Tickers(Findex);
            var LRlist = await Deserialize_Tickers(Lindex);

            int index = 0;
            for (int i = 0; i < FRlist.Length; i++)
            {
                for (int j = 0; j < LRlist.Length; j++)
                {
                    if ((FRlist[i].market == LRlist[j].market) && (FRlist[i].ask != "") && (FRlist[i].bid != "") && (LRlist[j].ask != "") && (LRlist[j].bid != ""))
                    {
                        comboBox3.Items.Add(FRlist[i].market);
                        if (FRlist[i].market.Contains(market))
                            index = comboBox3.Items.Count - 1;
                        dataGridView1.Rows.Add(1);
                        dataGridView1.Rows[dataGridView1.RowCount - 2].Cells[0].Value = FRlist[i].market;
                        dataGridView1.Rows[dataGridView1.RowCount - 2].Cells[1].Value = FRlist[i].bid;
                        dataGridView1.Rows[dataGridView1.RowCount - 2].Cells[2].Value = FRlist[i].ask;

                        dataGridView1.Rows[dataGridView1.RowCount - 2].Cells[3].Value = LRlist[j].bid;
                        dataGridView1.Rows[dataGridView1.RowCount - 2].Cells[4].Value = LRlist[j].ask;

                        dataGridView1.Rows[dataGridView1.RowCount - 2].Cells[5].Value = CalculateDiff(FRlist[i].bid, FRlist[i].ask, LRlist[j].bid, LRlist[j].ask);
                        dataGridView1.Rows[dataGridView1.RowCount - 2].Cells[6].Value = CalculateTasvieh(FRlist[i].bid, FRlist[i].ask, LRlist[j].bid, LRlist[j].ask);
                    }
                }
            }
            comboBox3.SelectedIndex = index;
            if (buttonFlag)
                button2.Enabled = true;
        }

        private async Task<Response_List[]> Deserialize_Tickers(int index)
        {
            string result;
            List<Response_List> list = new List<Response_List>();
            switch (index)
            {
                case 0://Coinex
                    result = await clientEx[index].GetStringAsync("v1/market/ticker/all");
                    var rlist = JsonConvert.DeserializeObject<COINEX_Response_List>(result);
                    var tickers = JsonConvert.DeserializeObject<Dictionary<string, COINEX_Response_List_L3>>(rlist.data.ticker.ToString());
                    var names = tickers.Keys.ToList();
                    foreach (var item in names)
                    {
                        var temp = new Response_List();
                        temp.market = item.Replace("USDT", "-USDT");
                        temp.ask = tickers[item].sell;
                        temp.bid = tickers[item].buy;
                        list.Add(temp);
                    }
                    return list.ToArray();
                case 1://Okx
                    result = await clientEx[index].GetStringAsync("/api/v5/market/tickers?instType=SPOT");
                    var tickers1 = JsonConvert.DeserializeObject<OKX_Response_List>(result);
                    foreach (var item in tickers1.data)
                    {
                        var temp = new Response_List();
                        temp.market = item.instId;
                        temp.ask = item.askPx;
                        temp.bid = item.bidPx;
                        list.Add(temp);
                    }
                    return list.ToArray();
                case 2://KuCoin
                    result = await clientEx[index].GetStringAsync("/api/v1/market/allTickers");
                    var tickers2 = JsonConvert.DeserializeObject<KUCOIN_Response_List>(result);
                    foreach (var item in tickers2.data.ticker)
                    {
                        var temp = new Response_List();
                        temp.market = item.symbol;
                        temp.ask = item.sell;
                        temp.bid = item.buy;
                        list.Add(temp);
                    }
                    return list.ToArray();
                case 3://Gate
                    result = await clientEx[index].GetStringAsync("/api/v4/spot/tickers");
                    var tickers3 = JsonConvert.DeserializeObject<GATE_Response_List_L2[]>(result);
                    foreach (var item in tickers3)
                    {
                        var temp = new Response_List();
                        temp.market = item.currency_pair.Replace("_", "-");
                        temp.ask = item.lowest_ask;
                        temp.bid = item.highest_bid;
                        list.Add(temp);
                    }
                    return list.ToArray();
                case 4://HuobiPro
                    result = await clientEx[index].GetStringAsync("/market/tickers");
                    var tickers4 = JsonConvert.DeserializeObject<HUOBIPRO_Response_List>(result);
                    foreach (var item in tickers4.data)
                    {
                        var temp = new Response_List();
                        temp.market = item.symbol.ToUpper().Replace("USDT", "-USDT");
                        temp.ask = item.ask;
                        temp.bid = item.bid;
                        list.Add(temp);
                    }
                    return list.ToArray();
                default:
                    return null;
            }
        }

        private string CalculateDiff(string buy, string sell, string bidPx, string askPx)
        {
            var dif1 = double.Parse(buy) / double.Parse(askPx) - 1;
            var dif2 = double.Parse(bidPx) / double.Parse(sell) - 1;
            if (dif1 > dif2)
            {
                dataGridView1.Rows[dataGridView1.RowCount - 2].Cells[1].Style.ForeColor = Color.Green;
                dataGridView1.Rows[dataGridView1.RowCount - 2].Cells[5].Style.ForeColor = Color.Green;
                return Math.Round(dif1 * 100, 2).ToString();
            }
            else
            {
                dataGridView1.Rows[dataGridView1.RowCount - 2].Cells[2].Style.ForeColor = Color.Green;
                dataGridView1.Rows[dataGridView1.RowCount - 2].Cells[4].Style.ForeColor = Color.Green;
                return Math.Round(dif2 * 100, 2).ToString();
            }
        }

        private string CalculateTasvieh(string buy, string sell, string bidPx, string askPx)
        {
            var dif1 = double.Parse(buy) / double.Parse(askPx) - 1;
            var dif2 = double.Parse(bidPx) / double.Parse(sell) - 1;
            if (dif1 > dif2)
            {
                var diff = double.Parse(sell) / double.Parse(bidPx) - 1;
                return Math.Round(diff * 100, 2).ToString();
            }
            else
            {
                var diff = double.Parse(askPx) / double.Parse(buy) - 1;
                return Math.Round(diff * 100, 2).ToString();
            }
        }

        private void m_BuildGrid(DataGridView dgv)
        {
            for (int i = 0; i < 7; i++)
            {
                var strTemp = "col" + i.ToString();
                dgv.Columns.Add(@strTemp, i.ToString());
                //pColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            dgv.Columns[0].HeaderText = "Market";

            dgv.Columns[1].HeaderText = "Buy_Price(" + comboBox1.SelectedItem + ")";
            dgv.Columns[2].HeaderText = "Sell_Price(" + comboBox1.SelectedItem + ")";

            dgv.Columns[3].HeaderText = "Buy_Price(" + comboBox2.SelectedItem + ")";
            dgv.Columns[4].HeaderText = "Sell_Price(" + comboBox2.SelectedItem + ")";

            dgv.Columns[5].HeaderText = "Diff(%)";
            dgv.Columns[6].HeaderText = "Tasvieh(%)";
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            SaveFormParams();
            button2.Enabled = false;
            buttonFlag = false;
            market = comboBox3.SelectedItem.ToString();
            Threshold = double.Parse(textBox1.Text);
            openDiff = double.Parse(textBox2.Text);
            closeDiff = double.Parse(textBox3.Text);
            amountT = double.Parse(textBox4.Text);
            sellParam = checkBox1.Checked;
            comboBox1.Enabled = false;
            comboBox2.Enabled = false;
            comboBox3.Enabled = false;
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = false;
            checkBox1.Enabled = false;
            maxDiff = -10000;
            minDiff = 10000;
            hFlag = lFlag = true;
            high[1] = high[0] = openDiff;
            low[1] = low[0] = closeDiff;
            if (Findex == 2 || Lindex == 2)
                await Kucoin_getPrecision(2, market);//2=Kucoin index
            if (Findex == 4 || Lindex == 4)
            {
                await HuobiPro_getMyAccount(4);//4=HuobiPro index
                await HuobiPro_getPrecision(4, market);//4=HuobiPro index
            }
            //Put_Order(Findex, market, "limit", "sell", 0, 0.000000001877);
            timer1.Enabled = true;
        }

        private void SaveFormParams()
        {
            string str = "";
            str += comboBox1.SelectedIndex.ToString() + Environment.NewLine;
            str += comboBox2.SelectedIndex.ToString() + Environment.NewLine;
            str += comboBox3.SelectedItem.ToString() + Environment.NewLine;
            str += textBox1.Text + Environment.NewLine;
            str += textBox2.Text + Environment.NewLine;
            str += textBox3.Text + Environment.NewLine;
            str += textBox4.Text + Environment.NewLine;
            File.WriteAllText("CodesForm.txt", str);
        }

        private async Task Kucoin_getPrecision(int index, string market)
        {
            var result = await clientEx[index].GetStringAsync("/api/v1/symbols");
            var rposition = JsonConvert.DeserializeObject<KUCOIN_Response_Precision>(result);
            foreach (var item in rposition.data)
            {
                if (item.symbol == market)
                {
                    Kucoin_Precision = item.baseIncrement.Split('.')[1].Count();
                    break;
                }
            }
        }

        private async Task HuobiPro_getMyAccount(int index)
        {
            try
            {
                var now = DateTime.UtcNow;
                var timems = now.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture);
                var str = "GET\napi.huobi.pro\n/v1/account/accounts\n";
                var url = ("AccessKeyId=" + Access_id[index] + "&SignatureMethod=HmacSHA256&SignatureVersion=2&Timestamp=" + timems).Replace(":", "%3A");
                str += url;
                var hash = SHA256_ComputeHash(str, Secret_key[index]);
                var result = await clientEx[index].GetStringAsync("/v1/account/accounts?" + url + "&Signature=" + hash);
                var rposition = JsonConvert.DeserializeObject<HUOBIPRO_Response_Account>(result);
                foreach (var item in rposition.data)
                {
                    if (item.type == "spot")
                    {
                        HuobiPro_AccountId = item.id;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("HuobiPro_getMyAccount Error: " + ex.ToString());
            }
        }

        private async Task HuobiPro_getPrecision(int index, string market)
        {
            try
            {
                var result = await clientEx[index].GetStringAsync("/v1/settings/common/market-symbols?symbols=" + market.Replace("-", "").ToLower());
                var rposition = JsonConvert.DeserializeObject<HUOBIPRO_Response_Precision>(result);
                HuobiPro_Precision = int.Parse(rposition.data[0].ap);
            }
            catch (Exception ex)
            {
                MessageBox.Show("HuobiPro_getPrecision Error: " + ex.ToString());
            }
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            Response_Depth FDepth, LDepth;
            try
            {
                FDepth = await Deserialize_Depth(Findex);
                LDepth = await Deserialize_Depth(Lindex);
            }
            catch (Exception ex)
            {
                Logger(alarmChatId, "timer1_Tick Error: Findex=" + Findex + " Lindex=" + Lindex + " market=" + market + Environment.NewLine + ex.ToString());
                return;
            }


            //Depth***********************
            int cnt = 0;
            double sumPrice = 0, sumAmountFirstBuy = 0, sumAmountFirstSell = 0, sumAmountLastBuy = 0, sumAmountLastSell = 0;
            double firstBuy, firstSell, lastBuy, lastSell;
            double firstBuyAvg, firstSellAvg, lastBuyAvg, lastSellAvg;
            while (sumPrice < amountT && cnt < depth)
            {
                sumAmountFirstBuy += double.Parse(FDepth.bids[cnt][1]);
                sumPrice += double.Parse(FDepth.bids[cnt][0]) * double.Parse(FDepth.bids[cnt][1]);
                cnt++;
            }
            firstBuyAvg = sumPrice / sumAmountFirstBuy;
            firstBuy = double.Parse(FDepth.bids[cnt - 1][0]);
            cnt = 0;
            sumPrice = 0;
            while (sumPrice < amountT && cnt < depth)
            {
                sumAmountFirstSell += double.Parse(FDepth.asks[cnt][1]);
                sumPrice += double.Parse(FDepth.asks[cnt][0]) * double.Parse(FDepth.asks[cnt][1]);
                cnt++;
            }
            firstSellAvg = sumPrice / sumAmountFirstSell;
            firstSell = double.Parse(FDepth.asks[cnt - 1][0]);
            cnt = 0;
            sumPrice = 0;
            while (sumPrice < amountT && cnt < depth)
            {
                sumAmountLastBuy += double.Parse(LDepth.bids[cnt][1]);
                sumPrice += double.Parse(LDepth.bids[cnt][0]) * double.Parse(LDepth.bids[cnt][1]);
                cnt++;
            }
            lastBuyAvg = sumPrice / sumAmountLastBuy;
            lastBuy = double.Parse(LDepth.bids[cnt - 1][0]);
            cnt = 0;
            sumPrice = 0;
            while (sumPrice < amountT && cnt < depth)
            {
                sumAmountLastSell += double.Parse(LDepth.asks[cnt][1]);
                sumPrice += double.Parse(LDepth.asks[cnt][0]) * double.Parse(LDepth.asks[cnt][1]);
                cnt++;
            }
            lastSellAvg = sumPrice / sumAmountLastSell;
            lastSell = double.Parse(LDepth.asks[cnt - 1][0]);
            //****************************
            var dif1 = (firstBuyAvg / lastSellAvg - 1) * 100;
            var dif2 = (lastBuyAvg / firstSellAvg - 1) * 100;
            if (dif1 < 0) dif1 = (lastSellAvg / firstBuyAvg - 1) * -100;
            if (dif2 < 0) dif2 = (firstSellAvg / lastBuyAvg - 1) * -100;

            CalculateHighLows(dif1, dif2);
            openDiff = double.Parse(textBox2.Text);
            closeDiff = double.Parse(textBox3.Text);

            if (sellParam)
            {
                var diff = dif2 * -1;
                maxDiff = (diff > maxDiff) ? diff : maxDiff;
                minDiff = (diff < minDiff) ? diff : minDiff;
                string str = "Buy in " + comboBox1.SelectedItem + Environment.NewLine + "Sell in " + comboBox2.SelectedItem + Environment.NewLine +
                    "Diff= %" + Math.Round(diff, 2).ToString() + Environment.NewLine;
                str += "maxDiff(%): " + Math.Round(maxDiff, 2).ToString() + Environment.NewLine;
                str += "minDiff(%): " + Math.Round(minDiff, 2).ToString() + Environment.NewLine;
                str += "D.Amnt." + comboBox1.SelectedItem + "Sell: " + sumAmountFirstSell.ToString() + Environment.NewLine;
                str += "D.Amnt." + comboBox2.SelectedItem + "Buy: " + sumAmountLastBuy.ToString() + Environment.NewLine;
                UpdateRichTextbox(richTextBox2, str);
                if (diff <= closeDiff)
                {
                    Put_Order(Findex, market, "limit", "buy", Math.Round((amountT / firstSellAvg), 10), firstSell);
                    Put_Order(Lindex, market, "limit", "sell", 0, lastBuy);
                    str = "Buy in " + comboBox1.SelectedItem + ": " + Math.Round(firstSell, 15).ToString("0." + new string('#', 339)) + Environment.NewLine;
                    str += "Sell in " + comboBox2.SelectedItem + ": " + Math.Round(lastBuy, 15).ToString("0." + new string('#', 339)) + Environment.NewLine;
                    str += "Amount($): " + amountT.ToString() + Environment.NewLine;
                    str += "Diff(%): " + Math.Round(diff, 2).ToString() + Environment.NewLine;
                    str += "maxDiff(%): " + Math.Round(maxDiff, 2).ToString() + Environment.NewLine;
                    str += "minDiff(%): " + Math.Round(minDiff, 2).ToString() + Environment.NewLine;
                    UpdateRichTextbox(richTextBox2, str);
                    Logger(alarmChatId, market + Environment.NewLine + str);
                    sellParam = false;
                    maxDiff = -10000;
                    minDiff = 10000;
                }
            }
            else
            {
                maxDiff = (dif1 > maxDiff) ? dif1 : maxDiff;
                minDiff = (dif1 < minDiff) ? dif1 : minDiff;
                string str = "Sell in " + comboBox1.SelectedItem + Environment.NewLine + "Buy in " + comboBox2.SelectedItem + Environment.NewLine +
                    "Diff= %" + Math.Round(dif1, 2).ToString() + Environment.NewLine;
                str += "maxDiff(%): " + Math.Round(maxDiff, 2).ToString() + Environment.NewLine;
                str += "minDiff(%): " + Math.Round(minDiff, 2).ToString() + Environment.NewLine;
                str += "D.Amnt."+ comboBox1.SelectedItem + "Buy: " + sumAmountFirstBuy.ToString() + Environment.NewLine;
                str += "D.Amnt."+ comboBox2.SelectedItem + "Sell: " + sumAmountLastSell.ToString() + Environment.NewLine;
                UpdateRichTextbox(richTextBox1, str);
                if (dif1 >= openDiff)
                {
                    Put_Order(Findex, market, "limit", "sell", 0, firstBuy);
                    Put_Order(Lindex, market, "limit", "buy", Math.Round((amountT / lastSellAvg), 10), lastSell);
                    str = "Sell in " + comboBox1.SelectedItem + ": " + Math.Round(firstBuy, 15).ToString("0." + new string('#', 339)) + Environment.NewLine;
                    str += "Buy in " + comboBox2.SelectedItem + ": " + Math.Round(lastSell, 15).ToString("0." + new string('#', 339)) + Environment.NewLine;
                    str += "Amount($): " + amountT.ToString() + Environment.NewLine;
                    str += "Diff(%): " + Math.Round(dif1, 2).ToString() + Environment.NewLine;
                    str += "maxDiff(%): " + Math.Round(maxDiff, 2).ToString() + Environment.NewLine;
                    str += "minDiff(%): " + Math.Round(minDiff, 2).ToString() + Environment.NewLine;
                    UpdateRichTextbox(richTextBox1, str);
                    Logger(alarmChatId, market + Environment.NewLine + str);
                    sellParam = true;
                    maxDiff = -10000;
                    minDiff = 10000;
                }
            }
        }
        
        private async Task<Response_Depth> Deserialize_Depth(int index)
        {
            string result;
            Response_Depth list = new Response_Depth();
            switch (index)
            {
                case 0://Coinex
                    result = await clientEx[index].GetStringAsync("v1/market/depth?market=" + market.Replace("-", "") + "&limit=" + depth + "&merge=0");
                    var rdepth = JsonConvert.DeserializeObject<COINEX_Response_Depth>(result);
                    list.asks = rdepth.data.asks;
                    list.bids = rdepth.data.bids;
                    return list;
                case 1://Okx
                    result = await clientEx[index].GetStringAsync("/api/v5/market/books?instId=" + market + "&sz=" + depth);
                    var rdepth1 = JsonConvert.DeserializeObject<OKX_Response_Depth>(result);
                    list.asks = rdepth1.data[0].asks;
                    list.bids = rdepth1.data[0].bids;
                    return list;
                case 2://KuCoin
                    result = await clientEx[index].GetStringAsync("/api/v1/market/orderbook/level2_20?symbol=" + market);
                    var rdepth2 = JsonConvert.DeserializeObject<KUCOIN_Response_Depth>(result);
                    list.asks = rdepth2.data.asks;
                    list.bids = rdepth2.data.bids;
                    return list;
                case 3://Gate
                    result = await clientEx[index].GetStringAsync("/api/v4/spot/order_book?currency_pair=" + market.Replace("-", "_"));
                    var rdepth3 = JsonConvert.DeserializeObject<GATE_Response_Depth>(result);
                    list.asks = rdepth3.asks;
                    list.bids = rdepth3.bids;
                    return list;
                case 4://HuobiPro
                    result = await clientEx[index].GetStringAsync("/market/depth?symbol=" + market.ToLower().Replace("-", "") + "&depth=" + depth + "&type=step0");
                    var rdepth4 = JsonConvert.DeserializeObject<HUOBIPRO_Response_Depth>(result);
                    list.asks = rdepth4.tick.asks;
                    list.bids = rdepth4.tick.bids;
                    return list;
                default:
                    return null;
            }
        }

        private void CalculateHighLows(double dif1, double dif2)
        {
            double diff;
            if ((dif1 < 0) && (dif2 < 0))
                return;
            else if (dif1 >= 0)
                diff = Math.Round(dif1, 2);
            else
                diff = Math.Round(dif2, 2) * -1;
            if (lFlag && ((high[0] - diff) > Threshold))
            {
                lFlag = false;
                hFlag = true;
                var val = high[1];
                if (high[0] < high[1]) val = high[0];
                UpdateTextbox(textBox2, Math.Floor(val).ToString());
                SaveFormParams();

                low[1] = low[0];
                low[0] = diff;
            }
            else if (hFlag && ((diff - low[0]) > Threshold))
            {
                hFlag = false;
                lFlag = true;
                var val = low[1];
                if (low[0] > low[1]) val = low[0];
                UpdateTextbox(textBox3, Math.Ceiling(val).ToString());
                SaveFormParams();

                high[1] = high[0];
                high[0] = diff;
            }
            if (diff > high[0])
            {
                high[0] = diff;
            }
            else if (diff < low[0])
            {
                low[0] = diff;
            }
        }

        private async void Put_Order(int index, string market, string type, string side, double amount, double price)
        {
            DateTime now;
            TimeSpan diff;
            string timesec, timems, str, hash, hash2, json, result, url;
            Dictionary<string, string> tradeParam;
            StringContent data;
            HttpResponseMessage response;
            Response res;
            double theAmount;
            try
            {
                switch (index)
                {
                    case 0://Coinex
                        theAmount = amount;
                        if (side == "sell")
                        {
                            theAmount = 0;
                            diff = DateTime.UtcNow - baseDate;
                            timems = ((long)diff.TotalMilliseconds).ToString();
                            str = "access_id=" + Access_id[index] + "&tonce=" + timems;
                            hash = MD5Hash(str + "&secret_key=" + Secret_key[index]);
                            clientEx[index].DefaultRequestHeaders.Clear();
                            clientEx[index].DefaultRequestHeaders.Add("Authorization", hash);
                            clientEx[index].DefaultRequestHeaders.Add("AccessId", Access_id[index]);
                            result = await clientEx[index].GetStringAsync("/v1/balance/info?" + str);
                            var rpositiontemp = JsonConvert.DeserializeObject<COINEX_Balance>(result);
                            var rposition = JsonConvert.DeserializeObject<Dictionary<string, COINEX_Balance_L2>>(rpositiontemp.data.ToString());
                            foreach (var item in rposition)
                            {
                                if (item.Key == market.Split('-')[0])
                                {
                                    theAmount = double.Parse(item.Value.available);
                                    break;
                                }
                            }
                        }
                        diff = DateTime.UtcNow - baseDate;
                        timems = ((long)diff.TotalMilliseconds).ToString();
                        str = "access_id=" + Access_id[index] + "&amount=" + theAmount.ToString("0." + new string('#', 339)) +
                            "&market=" + market.Replace("-", "") + "&price=" + price.ToString("0." + new string('#', 339)) +
                            "&tonce=" + timems + "&type=" + side;
                        tradeParam =
                            new Dictionary<string, string>()
                            {
                                {"access_id" , Access_id[index] },
                                {"amount", theAmount.ToString("0." + new string('#', 339)) },
                                {"market", market.Replace("-","") },
                                {"price", price.ToString("0." + new string('#', 339)) },
                                {"tonce" , timems},
                                {"type" , side}
                            };
                        json = JsonConvert.SerializeObject(tradeParam);
                        hash = MD5Hash(str + "&secret_key=" + Secret_key[index]);
                        clientEx[index].DefaultRequestHeaders.Clear();
                        clientEx[index].DefaultRequestHeaders.Add("authorization", hash);
                        data = new StringContent(json, Encoding.UTF8, "application/json");
                        response = await clientEx[index].PostAsync("v1/order/limit", data);
                        result = await response.Content.ReadAsStringAsync();
                        res = JsonConvert.DeserializeObject<Response>(result);
                        if (int.Parse(res.code.ToString()) > 0)
                            Logger(alarmChatId, comboBox1.Items[index] + " Put_Order Error: " + result.ToString());
                        break;
                    case 1://Okx
                        theAmount = amount;
                        if (side == "sell")
                        {
                            now = DateTime.UtcNow;
                            timems = now.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);
                            str = "/api/v5/account/balance?ccy=" + market.Split('-')[0];
                            hash = CreateToken(timems + "GET" + str, Secret_key[index]);
                            clientEx[index].DefaultRequestHeaders.Clear();
                            clientEx[index].DefaultRequestHeaders.Add("OK-ACCESS-KEY", Access_id[index]);
                            clientEx[index].DefaultRequestHeaders.Add("OK-ACCESS-SIGN", hash);
                            clientEx[index].DefaultRequestHeaders.Add("OK-ACCESS-TIMESTAMP", timems);
                            clientEx[index].DefaultRequestHeaders.Add("OK-ACCESS-PASSPHRASE", Pass[index]);
                            result = await clientEx[index].GetStringAsync(str);
                            var rposition1 = JsonConvert.DeserializeObject<OKX_Balance>(result);
                            theAmount = double.Parse(rposition1.data[0].details[0].availEq);
                        }
                        now = DateTime.UtcNow;
                        timems = now.ToString("yyyy-MM-dd'T'HH:mm:ss.fffK", CultureInfo.InvariantCulture);
                        tradeParam = new Dictionary<string, string>()
                          {
                            {"instId" , market },
                            {"tdMode", "cash" },
                            {"side", side },
                            {"ordType" , type},
                            {"sz" , theAmount.ToString("0." + new string('#', 339))},//considered Contract value
                            {"px" , price.ToString("0." + new string('#', 339))}
                          };
                        json = JsonConvert.SerializeObject(tradeParam);
                        str = timems + "POST" + "/api/v5/trade/order" + json;
                        hash = CreateToken(str, Secret_key[index]);
                        clientEx[index].DefaultRequestHeaders.Clear();
                        clientEx[index].DefaultRequestHeaders.Add("OK-ACCESS-KEY", Access_id[index]);
                        clientEx[index].DefaultRequestHeaders.Add("OK-ACCESS-SIGN", hash);
                        clientEx[index].DefaultRequestHeaders.Add("OK-ACCESS-TIMESTAMP", timems);
                        clientEx[index].DefaultRequestHeaders.Add("OK-ACCESS-PASSPHRASE", Pass[index]);
                        data = new StringContent(json, Encoding.UTF8, "application/json");
                        response = await clientEx[index].PostAsync("/api/v5/trade/order", data);
                        result = await response.Content.ReadAsStringAsync();
                        res = JsonConvert.DeserializeObject<Response>(result);
                        if (int.Parse(res.code.ToString()) > 0)
                            Logger(alarmChatId, comboBox1.Items[index] + " Put_Order Error: " + result.ToString());
                        break;
                    case 2://KuCoin size error
                        theAmount = amount;
                        if (side == "sell")
                        {
                            diff = DateTime.UtcNow - baseDate;
                            timems = ((long)(diff.TotalMilliseconds)).ToString();
                            str = timems + "GET" + "/api/v1/accounts?currency=" + market.Split('-')[0] + "&type=trade";
                            hash = CreateToken(str, Secret_key[index]);
                            hash2 = CreateToken(Pass[index], Secret_key[index]);
                            clientEx[index].DefaultRequestHeaders.Clear();
                            clientEx[index].DefaultRequestHeaders.Add("KC-API-KEY", Access_id[index]);
                            clientEx[index].DefaultRequestHeaders.Add("KC-API-SIGN", hash);
                            clientEx[index].DefaultRequestHeaders.Add("KC-API-TIMESTAMP", timems);
                            clientEx[index].DefaultRequestHeaders.Add("KC-API-PASSPHRASE", hash2);
                            clientEx[index].DefaultRequestHeaders.Add("KC-API-KEY-VERSION", "2");
                            result = await clientEx[index].GetStringAsync("/api/v1/accounts?currency=" + market.Split('-')[0] + "&type=trade");
                            var rposition2 = JsonConvert.DeserializeObject<KUCOIN_Balance>(result);
                            theAmount = double.Parse(rposition2.data[0].available);
                        }
                        theAmount = Math.Floor(theAmount * Math.Pow(10, Kucoin_Precision)) / Math.Pow(10, Kucoin_Precision);
                        diff = DateTime.UtcNow - baseDate;
                        timems = ((long)(diff.TotalMilliseconds)).ToString();
                        tradeParam =
                          new Dictionary<string, string>()
                          {
                              {"clientOid" , timems },
                              {"symbol" , market },
                              {"type" , type },
                              {"side" , side },
                              {"size" , theAmount.ToString("0." + new string('#', 339)) },
                              {"price" , price.ToString("0." + new string('#', 339)) }
                          };
                        json = JsonConvert.SerializeObject(tradeParam);
                        str = timems + "POST" + "/api/v1/orders" + json;
                        hash = CreateToken(str, Secret_key[index]);
                        hash2 = CreateToken(Pass[index], Secret_key[index]);
                        clientEx[index].DefaultRequestHeaders.Clear();
                        clientEx[index].DefaultRequestHeaders.Add("KC-API-KEY", Access_id[index]);
                        clientEx[index].DefaultRequestHeaders.Add("KC-API-SIGN", hash);
                        clientEx[index].DefaultRequestHeaders.Add("KC-API-TIMESTAMP", timems);
                        clientEx[index].DefaultRequestHeaders.Add("KC-API-PASSPHRASE", hash2);
                        clientEx[index].DefaultRequestHeaders.Add("KC-API-KEY-VERSION", "2");
                        data = new StringContent(json, Encoding.UTF8, "application/json");
                        response = await clientEx[index].PostAsync("/api/v1/orders", data);
                        result = await response.Content.ReadAsStringAsync();
                        res = JsonConvert.DeserializeObject<Response>(result);
                        if (int.Parse(res.code.ToString()) != 200000)
                            Logger(alarmChatId, comboBox1.Items[index] + " Put_Order Error: " + res.msg.ToString());
                        break;
                    case 3://Gate
                        theAmount = amount;
                        if (side == "sell")
                        {
                            diff = DateTime.UtcNow - baseDate;
                            timesec = ((long)diff.TotalSeconds).ToString();
                            hash = getHashSha512("");
                            str = "GET" + "\n" + "/api/v4/spot/accounts" + "\n" + "currency=" + market.Split('-')[0] + "\n" + hash + "\n" + timesec;
                            hash2 = SHA512_ComputeHash(str, Secret_key[index]);
                            clientEx[index].DefaultRequestHeaders.Clear();
                            clientEx[index].DefaultRequestHeaders.Add("Accept", "application/json");
                            clientEx[index].DefaultRequestHeaders.Add("KEY", Access_id[index]);
                            clientEx[index].DefaultRequestHeaders.Add("Timestamp", timesec);
                            clientEx[index].DefaultRequestHeaders.Add("SIGN", hash2);
                            result = await clientEx[index].GetStringAsync("/api/v4/spot/accounts?" + "currency=" + market.Split('-')[0]);
                            var rposition3 = JsonConvert.DeserializeObject<GATE_Balance[]>(result);
                            theAmount = double.Parse(rposition3[0].available);
                        }
                        diff = DateTime.UtcNow - baseDate;
                        timesec = ((long)diff.TotalSeconds).ToString();
                        tradeParam =
                          new Dictionary<string, string>()
                          {
                              {"currency_pair" , market.Replace("-","_") },
                              {"type" , type },
                              {"side" , side },
                              {"amount" , theAmount.ToString("0." + new string('#', 339)) },
                              {"price" , price.ToString("0." + new string('#', 339)) }
                          };
                        json = JsonConvert.SerializeObject(tradeParam);
                        hash = getHashSha512(json);
                        str = "POST" + "\n" + "/api/v4/spot/orders" + "\n\n" + hash + "\n" + timesec;
                        hash2 = SHA512_ComputeHash(str, Secret_key[index]);
                        clientEx[index].DefaultRequestHeaders.Clear();
                        clientEx[index].DefaultRequestHeaders.Add("Accept", "application/json");
                        clientEx[index].DefaultRequestHeaders.Add("KEY", Access_id[index]);
                        clientEx[index].DefaultRequestHeaders.Add("Timestamp", timesec);
                        clientEx[index].DefaultRequestHeaders.Add("SIGN", hash2);
                        data = new StringContent(json, Encoding.UTF8, "application/json");
                        response = await clientEx[index].PostAsync("/api/v4/spot/orders", data);
                        result = await response.Content.ReadAsStringAsync();
                        res = JsonConvert.DeserializeObject<Response>(result);
                        if ((int)response.StatusCode != 201)
                            Logger(alarmChatId, comboBox1.Items[index] + " Put_Limit_Order Error: " + result);
                        break;
                    case 4://HuobiPro
                        theAmount = amount;
                        if (side == "sell")
                        {
                            theAmount = 0;
                            now = DateTime.UtcNow;
                            timems = now.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture);
                            str = "GET\napi.huobi.pro\n/v1/account/accounts/" + HuobiPro_AccountId + "/balance\n";
                            url = ("AccessKeyId=" + Access_id[index] + "&SignatureMethod=HmacSHA256&SignatureVersion=2&Timestamp=" + timems).Replace(":", "%3A");
                            str += url;
                            hash = SHA256_ComputeHash(str, Secret_key[index]);
                            result = await clientEx[index].GetStringAsync("/v1/account/accounts/" + HuobiPro_AccountId + "/balance?" + url + "&Signature=" + hash);
                            var rposition4 = JsonConvert.DeserializeObject<HUOBIPRO_Response_Balance>(result);
                            foreach (var item in rposition4.data.list)
                            {
                                if (item.currency.ToUpper() == market.Split('-')[0])
                                {
                                    theAmount = double.Parse(item.balance);
                                    break;
                                }
                            }
                        }
                        theAmount = Math.Floor(theAmount * Math.Pow(10, HuobiPro_Precision)) / Math.Pow(10, HuobiPro_Precision);
                        now = DateTime.UtcNow;
                        timems = now.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture);
                        tradeParam = new Dictionary<string, string>()
                          {
                            {"account-id", HuobiPro_AccountId },
                            {"symbol" , market.Replace("-","").ToLower() },
                            {"type", side + "-" + type },
                            {"amount" , theAmount.ToString("0." + new string('#', 339))},//considered Contract value
                            {"price" , price.ToString("0." + new string('#', 339))}
                          };
                        json = JsonConvert.SerializeObject(tradeParam);
                        str = "POST\napi.huobi.pro\n/v1/order/orders/place\n";
                        url = ("AccessKeyId=" + Access_id[index] + "&SignatureMethod=HmacSHA256&SignatureVersion=2&Timestamp=" + timems).Replace(":", "%3A");
                        str += url;
                        hash = SHA256_ComputeHash(str, Secret_key[index]);
                        data = new StringContent(json, Encoding.UTF8, "application/json");
                        response = await clientEx[index].PostAsync("/v1/order/orders/place?" + url + "&Signature=" + hash, data);
                        result = await response.Content.ReadAsStringAsync();
                        res = JsonConvert.DeserializeObject<Response>(result.Replace("err-msg","msg"));
                        if (res.status.ToString() != "ok")
                            Logger(alarmChatId, comboBox1.Items[index] + " Put_Order Error: " + res.msg.ToString());
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger(alarmChatId, comboBox1.Items[index] + " Put_Order Error: " + Environment.NewLine + ex.ToString());
            }
        }

        private static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text  
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it  
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString().ToUpper();
        }

        private string CreateToken(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }

        public static string getHashSha512(string text)
        {
            byte[] bytes = ASCIIEncoding.ASCII.GetBytes(text);
            SHA512Managed hashstring = new SHA512Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = string.Empty;
            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }
            return hashString;
        }

        public static string SHA512_ComputeHash(string text, string secretKey)
        {
            var hash = new StringBuilder();
            byte[] secretkeyBytes = Encoding.UTF8.GetBytes(secretKey);
            byte[] inputBytes = Encoding.UTF8.GetBytes(text);
            using (var hmac = new HMACSHA512(secretkeyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }

        public static string SHA256_ComputeHash(string text, string secretKey)
        {
            var hash = new StringBuilder();
            byte[] secretkeyBytes = Encoding.UTF8.GetBytes(secretKey);
            byte[] inputBytes = Encoding.UTF8.GetBytes(text);
            byte[] hashValue;
            using (var hmac = new HMACSHA256(secretkeyBytes))
            {
                hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return Uri.EscapeDataString(Convert.ToBase64String(hashValue));
        }

        delegate void UpdateRichTextboxCallback(RichTextBox item, string str);
        public void UpdateRichTextbox(RichTextBox item, string str)
        {
            if (this.richTextBox1.InvokeRequired)
            {
                var d = new UpdateRichTextboxCallback(UpdateRichTextbox);
                this.Invoke(d, new object[] { item, str });
            }
            else if (this.richTextBox2.InvokeRequired)
            {
                var d = new UpdateRichTextboxCallback(UpdateRichTextbox);
                this.Invoke(d, new object[] { item, str });
            }
            else
            {
                item.Text = str;
            }
        }

        delegate void UpdateTextboxCallback(TextBox item, string str);
        public void UpdateTextbox(TextBox item, string str)
        {
            if (this.textBox2.InvokeRequired)
            {
                var d = new UpdateTextboxCallback(UpdateTextbox);
                this.Invoke(d, new object[] { item, str });
            }
            else if (this.textBox3.InvokeRequired)
            {
                var d = new UpdateTextboxCallback(UpdateTextbox);
                this.Invoke(d, new object[] { item, str });
            }
            else
            {
                item.Text = str;
            }
        }

        private async Task Logger(string chatId, string s1)
        {
            try
            {
                await clientTelegram.GetStringAsync("https://api.telegram.org/bot" + "2135050880:AAGNiAJdwagxVP4Y3LBsHYqgBFUxk0Hnq8A"
                            + "/sendMessage?chat_id=" + chatId
                            + "&text=" + owner + Environment.NewLine
                            + DateTime.Now.ToString() + Environment.NewLine + s1);
            }
            catch (Exception ex) { }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button2.Enabled = false;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            button2.Enabled = false;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            comboBox1.SelectionLength = 0;
            comboBox2.SelectionLength = 0;
            comboBox3.SelectionLength = 0;
        }
    }
}
