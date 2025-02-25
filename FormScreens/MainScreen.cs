﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;


namespace borsaProjesi
{
    public partial class MainScreen : Form
    {
      
        private string sorgu = "";
        public MainScreen()
        {
            InitializeComponent();    
        }
        private void MainScreen_Load(object sender, EventArgs e)
        {
            MainScreenOnLoad();
      
        }
        public void MainScreenOnLoad()
        {
            Singleton.Instance.islem.SiparistenSatinAl();

            ParabirimiComboDoldur();
            Anasayfa_Tarih.Text = "Tarih : " + DateTime.Parse(DateTime.Now.ToShortDateString()).ToShortDateString();// tarih anasayfa
            Profil_Tarih.Text = "Tarih : " + DateTime.Parse(DateTime.Now.ToShortDateString()).ToShortDateString();// tarih profilim

            listeleCombo.Text = "Tüm Satışlarım";
            KendiSatislarim_CheckBox.Checked = true;
            if (Singleton.Instance.currentUser.Cinsiyet == "Kadın")
            {
                kadinimage.Visible = true;
                kadinimage2.Visible = true;
            }
            else
            {
                kadinimage.Visible = false;
                kadinimage2.Visible = false;
            }
            urunler_listbox.Items.Clear();
            satinalgrpbox.Visible = false;
            ButonVisibleControl();
            GuncelProfil();
        
        }
        private void ParabirimiComboDoldur()
        {
            Parabirimleri_combo.Items.Clear();
            Parabirimleri_combo.Items.Add("TL");// Türk lirası
            Parabirimleri_combo.Items.Add("USD");// ABD Doları
            Parabirimleri_combo.Items.Add("EUR");//Euro
            Parabirimleri_combo.Items.Add("CHF");// İsviçre frangı
            Parabirimleri_combo.Items.Add("DKK"); // Danimarka kronu
            Parabirimleri_combo.Items.Add("GBP");// İngiliz Strelini
        }
        public void GuncelProfil()
        {
            puserid_label.Text = Singleton.Instance.currentUser.UserID.ToString();
            pusername_label.Text = Singleton.Instance.currentUser.UserName;
            puserpass_txt.Text = Singleton.Instance.currentUser.UserPass;

            profilad_label.Text = Singleton.Instance.currentUser.Ad;
            psoyad_label.Text = Singleton.Instance.currentUser.SoyAd;
            ptcno.Text = Singleton.Instance.currentUser.Tc.ToString();
            ptelefonno_txt.Text = Singleton.Instance.currentUser.Telefon.ToString();
            padres_txt.Text = Singleton.Instance.currentUser.Adres;
            p_emailtxt.Text = Singleton.Instance.currentUser.Eposta;
            p_unvan.Text = Singleton.Instance.currentUser.Unvan;
            uyelik_turu.Text = Singleton.Instance.currentUser.Unvan;

            //***********
            main_ad.Text = Singleton.Instance.currentUser.Ad;
            main_soyad.Text = Singleton.Instance.currentUser.SoyAd;
            bakiye_label.Text = Singleton.Instance.currentUser.Bakiye + " ₺";
            geciciBakiye_label.Text = Singleton.Instance.currentUser.GeciciBakiye +" "+ Singleton.Instance.currentUser.ParaBirimi;
            bakiyelabel2_txt.Text = bakiye_label.Text;
            gecicibakiye2label_txt.Text = geciciBakiye_label.Text;
            kacadetalmak_txt.Text = "";
            // güncel tarihler
          
        }

        private void ButonVisibleControl()
        {
            if (Singleton.Instance.currentUser.Unvan == "SATICI")
            {
                sorgu = "SELECT * FROM showUruns WHERE UrunSatici='" + Singleton.Instance.currentUser.UserName + "'";
                Urunekle_datagrid.DataSource = Singleton.Instance.islem.Showdatabases(sorgu);
                KendiSatislarim_CheckBox.Visible = true;
                CheckBoxcontrol();
                TxtDuzenle(false);
            }
            else
            {
                sorgu = "SELECT * FROM showUruns WHERE urunOnay='" + "Onaylandı" + "' AND UrunMiktari>0 ";
                Secereksatinal_datagrid.DataSource = Singleton.Instance.islem.Showdatabases(sorgu);
                KendiSatislarim_CheckBox.Visible = false;
                TxtDuzenle(true);
            }

        }
        // list box doldururuluyor
        private void OtosatinalPageLoad()
        {
            ListBoxDoldur();
            ListBoxControlEt();
        }
        public void ListBoxDoldur() => Singleton.Instance.islem.ListBoxDoldur(urunler_listbox);
        public void ListBoxControlEt()//Eğer aynı ürün isminden birden fazla varsa teke indiriyor
        {
            int x;
            do
            {
              x = 0;
                for (int i = 0; i < urunler_listbox.Items.Count; i++)
                {
                    for (int j = i + 1; j < urunler_listbox.Items.Count; j++)
                    {
                        if (urunler_listbox.Items[i].ToString().ToLower() == urunler_listbox.Items[j].ToString().ToLower())
                        {
                            urunler_listbox.Items.RemoveAt(i);
                            x++;
                        }

                    }
                }
            } while (x >0);
            
        }

        // Anasayfa İşlemleri Page-1
        private void Urunler_listbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (satinalgrpbox.Visible == false)
                satinalgrpbox.Visible = true;
            kacadetalmak_txt.Text = "";
            sorgu = "SELECT * FROM UrunGoster WHERE UrunName='" + urunler_listbox.SelectedItem.ToString() + "'  AND UrunMiktari>0  AND NOT UrunSatici='" + Singleton.Instance.currentUser.UserName + "'";
            urunusatanlar_datagrid.DataSource = Singleton.Instance.islem.Showdatabases(sorgu);
            adet_lbl.Text = urunusatanlar_datagrid.Rows[0].Cells["urunBirimi"].FormattedValue.ToString();
        }

        private void Otosatinal_btn_Click(object sender, EventArgs e)
        {
            if (SayiMi(kacadetalmak_txt.Text))
            {
                double toplampara = Singleton.Instance.islem.GetToplamPara(urunler_listbox.SelectedItem.ToString(), Convert.ToInt32(kacadetalmak_txt.Text));
                double muhasebeciHarac = toplampara / 100;
                double olmasiGerekenBakiye = toplampara + muhasebeciHarac;
                double toplamadet = Singleton.Instance.islem.GetUrunToplamAdet(urunler_listbox.SelectedItem.ToString());
                if (Convert.ToInt32(kacadetalmak_txt.Text) > toplamadet)
                {
                    MessageBox.Show("Magazının toplam stogunda bu kadar ürün yok,girebileceginiz maksimum deger:" + toplamadet + " " + adet_lbl.Text + " dir");
                }
                else if (Singleton.Instance.currentUser.Bakiye < olmasiGerekenBakiye)
                {
                    MessageBox.Show("Bu üründen " + kacadetalmak_txt.Text + " " + adet_lbl.Text + " almak için yeterli bakiyeniz bulunmamaktaıdır", "Bakiye Yetmiyorr");
                }
                else if (Singleton.Instance.currentUser.Bakiye >= olmasiGerekenBakiye)
                {
                    Singleton.Instance.islem.Otosatinal(urunler_listbox.SelectedItem.ToString(), Convert.ToInt32(kacadetalmak_txt.Text),muhasebeciHarac);
                }

            }
            else if (kacadetalmak_txt.Text == "")
                MessageBox.Show("Lütfen Kac Adet Almak istediginizi giriniz", "Tüm alanları doldurun");
            else
            {
                MessageBox.Show("Lütfen Geçerli Bir Deger Giriniz", "Syntax hata ");
            }
        }
        private void Hangiurunu_listele_Click(object sender, EventArgs e) => OtosatinalPageLoad();
     
        private void Titlebakiye_label_MouseHover(object sender, EventArgs e)
        {
            geciciBakiye_label.Visible = true;
            unonay_label.Visible = true;
        }

        private void Titlebakiye_label_MouseLeave(object sender, EventArgs e)
        {
            geciciBakiye_label.Visible = false;
            unonay_label.Visible = false;
        }
       
        private void Btn_arttir_Click(object sender, EventArgs e) => IncreaseOrDecrease(true);    
        private void Btn_azalt_Click(object sender, EventArgs e) => IncreaseOrDecrease(false);
        private void IncreaseOrDecrease(bool arttir)
        {
            int x = Convert.ToInt32(adetarttir_label.Text);
            int fiyat = Convert.ToInt32(fiyat_label.Text);
            int odecenecektutar;
            if (arttir == false && x > 0)
                x--;    
            else if (arttir == true && x < Convert.ToInt32(adetsayisi_label.Text))
                x++;
            else if(x == Convert.ToInt32(adetsayisi_label.Text))
                MessageBox.Show("Bu SATICI için maksimum adet sayısına ulaştınız", "Stok Yetmezligi");
            
            odecenecektutar = fiyat * x;
            adetarttir_label.Text = x.ToString();
            odenecekttutar_label.Text = odecenecektutar.ToString();
        }

        private void Satinal_btn_Click(object sender, EventArgs e)
        {
            double toplampara = Convert.ToDouble(odenecekttutar_label.Text);
            double muhasebeci = Convert.ToDouble(odenecekttutar_label.Text)/100;
            double muhasebeciVeToplampara =  toplampara+muhasebeci;
            string x;
            if (urunsaticilabel.Text == Singleton.Instance.currentUser.UserName)
            {
                MessageBox.Show("Kendi Ürününüzü Satın Alamazsınız", "Hatalı ürün seçimi");
            }
            else
            {
                DialogResult eminmisin = MessageBox.Show("Bu ürünü almak için istedigine eminmisin Toplam Fiyat:" + muhasebeciVeToplampara.ToString() + " ₺", "Emin misin ?", MessageBoxButtons.YesNo);
                if (eminmisin == DialogResult.Yes)
                {
                    if (muhasebeciVeToplampara > Singleton.Instance.currentUser.Bakiye)
                    {
                        double eksikbakiye = muhasebeciVeToplampara - Convert.ToInt32(Singleton.Instance.currentUser.Bakiye) ;
                        DialogResult dialogResult = MessageBox.Show("Bu ürünü almak için bakiyeniz yetersiz bu ürün için eksik bakiyeniz :" + eksikbakiye.ToString() + "₺ dir. Eksik Bakiyeyi Yüklemek istermisiniz", "Bakiye Yetersizliği !!!", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            if (Singleton.Instance.currentUser.BakiyeOnay == "Onaylanmadı")
                            {
                                MessageBox.Show("Henüz onaylanmamış bakiyeniz oldugu için bakiye yükleme işlemi gerçekleştirilemiyor", "Bakiye Yükleme Hata");
                            }
                            else
                            {                  // Bakiye yükle
                                Singleton.Instance.islem.GecicibakiyeGuncelle(eksikbakiye,"TL");
                            }
                        }
                    }
                    else if (muhasebeciVeToplampara > 0 && adetsayisi_label.Text != "0")
                    {
                        x = adetarttir_label.Text + " " + urunbirimilabel.Text + " " + urunadilabel.Text + " vermiş oldugunuz sipariş yola çıkmıştır en yakın zamanda adresinizde olacaktır  ADRES:  " + Singleton.Instance.currentUser.Adres;
                        Singleton.Instance.islem.UrunSatinAl(barkodno_label.Text, urunsaticilabel.Text, Convert.ToInt32(adetarttir_label.Text), Convert.ToInt32(odenecekttutar_label.Text), x, muhasebeci);
                    }
                    else if (adetsayisi_label.Text == "0")
                    {
                        MessageBox.Show("Bu urunden," + urunsaticilabel.Text + " SATICISINDA malesef kalmamış", "Stok Hata");
                    }
                    MainScreenOnLoad();

                }
                else
                {
                    MessageBox.Show("İşlem iptal edildi");
                }
            }

        }
     

        // Profil İşlemleri Page-2*---------------------------------------------------------------------------------------------------------------------
        private void Bakiye_ekle_btn_Click(object sender, EventArgs e)
        {

            if (bakiyeekle_ext.Text == "" || Parabirimleri_combo.Text == "") 
            {
                MessageBox.Show("Lütfen Eklemek istediginiz tutarı ve para birimini giriniz");
            }
            else
            {
                if (Singleton.Instance.currentUser.BakiyeOnay == "Onaylandı")
                {
                    Singleton.Instance.islem.GecicibakiyeGuncelle(Convert.ToInt32(bakiyeekle_ext.Text),Parabirimleri_combo.Text);
                    ParabirimiComboDoldur();
                    bakiyeekle_ext.Text = "";
                    MainScreenOnLoad();
                }
                else
                {
                    MessageBox.Show("Bir Onceki bakiye yükleme işleminiz henüz onaylanmadıgı için işleminizi gerçekleştiremiyoruz", "Bakiye Yükleme Hata");
                }
            }


        }
       
        private void Update_btn_Click(object sender, EventArgs e)
        {
            if (puserpass_txt.Text == ""||p_emailtxt.Text==""||padres_txt.Text==""||ptelefonno_txt.Text=="")
            {
                MessageBox.Show("Guncellenek alanlar boş bırakılamaz");
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("Güncellemek istediginize eminmisiniz  ?", "Kullanıcı Güncellensin mi ?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    //yes

                    Singleton.Instance.islem.UserUpdate(puserpass_txt, p_emailtxt, padres_txt, ptelefonno_txt);
                   
                    MessageBox.Show("Güncelleme işlemi tamamlandı.");
                }
                else if (dialogResult == DialogResult.No)
                {
                    //no
                    MessageBox.Show("Güncelleme işlemi iptal edildi.");

                }
            }
            GuncelProfil();
        }

        // Urun İşlemleri Page -3 *---------------------------------------------------------------------------------------------------------------------


        private void BenimSattiklarim_btn_Click(object sender, EventArgs e) => ListeleComboBoxControl();
      
        private void Add_urun_btn_Click(object sender, EventArgs e)
        {
            Singleton.Instance.islem.UrunEkle(add_barkodno, urunbirimibox, add_urunfiyati, add_urunmiktari, add_urunname, add_urunturu, Singleton.Instance.currentUser.UserName, groupBox1);
            MessageBox.Show("Urun eklendi admin onayı bekleniyor", "Urun ekleme Başarılı");
            ListeleComboBoxControl();
        }

        private void Urun_guncelle_btn_Click(object sender, EventArgs e)
        {
            if (SayiMi(g_urunfiyati.Text) && SayiMi(g_urunmiktari.Text))
            {
             
                if (Convert.ToInt32(g_urunfiyati.Text) <= 0 || Convert.ToInt32(g_urunmiktari.Text) <= 0)
                {
                    MessageBox.Show("Lütfen 0 dan farklı pozitif sayılar giriniz.", "Hatalı urun güncellemesi");
                }
                else
                {
                   
                    Singleton.Instance.islem.UrunUpdate(g_urunfiyati, g_urunmiktari, g_urunbarkod.Text,groupBox1);
                    MessageBox.Show("Urun guncellendi admin onayı bekleniyor","Urun güncelleme Başarılı");
               
                }
            }
            else
            {
                MessageBox.Show("Lütfen Geçerli Bir deger girin");
            }
            ListeleComboBoxControl();
        }
       
        // Bool Sayimi 
        static bool SayiMi(string deger)
        {
            try
            {
                int.Parse(deger);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        // Fonksiyonlar
      

        private void TxtDuzenle(bool x)
        {
            // 1213; 498

            if (x == true)
            {
                txt_control.Visible = true;
                txt_control.Dock = DockStyle.Fill;
            }
            if (x == false)
                txt_control.Visible = false;
        }
        private void CheckBoxcontrol()
        {

            if (KendiSatislarim_CheckBox.Checked == true)
            {           
                sorgu = "SELECT * FROM showUruns WHERE urunOnay='" + "Onaylandı" + "' AND UrunMiktari>0 ";
                Secereksatinal_datagrid.DataSource = Singleton.Instance.islem.Showdatabases(sorgu);
            }

            if (KendiSatislarim_CheckBox.Checked == false)
            {
                sorgu = "SELECT * FROM showUruns WHERE urunOnay='" + "Onaylandı" + "' AND UrunMiktari>0 AND NOT UrunSatici='" + Singleton.Instance.currentUser.UserName + "'";
                Secereksatinal_datagrid.DataSource = Singleton.Instance.islem.Showdatabases(sorgu);

            }
        }
        private void ListeleComboBoxControl()
        {
            tumsatislarim_label.Text = listeleCombo.Text;
            sorgu = "SELECT * FROM showUruns WHERE UrunSatici='" + Singleton.Instance.currentUser.Ad + "'";     
            if (listeleCombo.Text == "Tüm Satışlarım")
                sorgu = "SELECT * FROM showUruns WHERE UrunSatici='" + Singleton.Instance.currentUser.Ad + "'";
            if (listeleCombo.Text == "Onaylanmış Satışlarım")
                sorgu = "SELECT * FROM showUruns WHERE UrunSatici='" + Singleton.Instance.currentUser.Ad + "' AND urunonay='" + "Onaylandı" + "'";
            if (listeleCombo.Text == "Onaylanmamış Satışlarım")
                sorgu = "SELECT * FROM showUruns WHERE UrunSatici='" + Singleton.Instance.currentUser.Ad + "' AND urunonay='" + "Onaylanmadı" + "'";
            Urunekle_datagrid.DataSource = Singleton.Instance.islem.Showdatabases(sorgu);
        }

        private void KendiSatislarim_CheckBox_CheckedChanged(object sender, EventArgs e) => CheckBoxcontrol();

        private void Secereksatinal_datagrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Secereksatinal_datagrid.CurrentRow.Selected = true;
            barkodno_label.Text = Secereksatinal_datagrid.Rows[e.RowIndex].Cells["urunBarkodNo"].FormattedValue.ToString();
            fiyat_label.Text = Secereksatinal_datagrid.Rows[e.RowIndex].Cells["UrunFiyati"].FormattedValue.ToString();
            adetsayisi_label.Text = Secereksatinal_datagrid.Rows[e.RowIndex].Cells["UrunMiktari"].FormattedValue.ToString();
            urunsaticilabel.Text = Secereksatinal_datagrid.Rows[e.RowIndex].Cells["UrunSatici"].FormattedValue.ToString();
            odenecekttutar_label.Text = Secereksatinal_datagrid.Rows[e.RowIndex].Cells["UrunFiyati"].FormattedValue.ToString();
            urunbirimilabel.Text = Secereksatinal_datagrid.Rows[e.RowIndex].Cells["UrunBirimi"].FormattedValue.ToString();
            urunadilabel.Text = Secereksatinal_datagrid.Rows[e.RowIndex].Cells["UrunName"].FormattedValue.ToString();
            if (Convert.ToInt32(adetsayisi_label.Text) > 0)
                adetarttir_label.Text = "1";
            else
                adetarttir_label.Text = "0";
            btn_arttir.Visible = true;
            btn_azalt.Visible = true;
        }

        private void Urunekle_datagrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Urunekle_datagrid.CurrentRow.Selected = true;
            g_urunid.Text = Urunekle_datagrid.Rows[e.RowIndex].Cells["UrunID"].FormattedValue.ToString();
            g_urunname.Text = Urunekle_datagrid.Rows[e.RowIndex].Cells["UrunName"].FormattedValue.ToString();
            g_urunmiktari.Text = Urunekle_datagrid.Rows[e.RowIndex].Cells["UrunMiktari"].FormattedValue.ToString();
            g_urunbirimi.Text = Urunekle_datagrid.Rows[e.RowIndex].Cells["urunBirimi"].FormattedValue.ToString();
            g_urunfiyati.Text = Urunekle_datagrid.Rows[e.RowIndex].Cells["UrunFiyati"].FormattedValue.ToString();
            g_urunturu.Text = Urunekle_datagrid.Rows[e.RowIndex].Cells["UrunTuru"].FormattedValue.ToString();
            g_urunbarkod.Text = Urunekle_datagrid.Rows[e.RowIndex].Cells["urunBarkodNo"].FormattedValue.ToString();
        }

      
        private void Back_btn_Click(object sender, EventArgs e) => Singleton.Instance.ChangeScreen(this, Singleton.Instance.login);

        private void SiparisVer_btn_MouseHover(object sender, EventArgs e) => tekfiyat_bottom_label.Visible = true;

        private void SiparisVer_btn_MouseLeave(object sender, EventArgs e) => tekfiyat_bottom_label.Visible = false;

        private void SiparisVer_btn_Click(object sender, EventArgs e)
        {
            Singleton.Instance.siparisVer.SiparisVerOnLoad();
            Singleton.Instance.ChangeScreen(this, Singleton.Instance.siparisVer);
        }

        private void GecmisAlısverisListem_Click(object sender, EventArgs e)
        {
            Singleton.Instance.alisverisListele.SiparislerimOnLoad();
            Singleton.Instance.ChangeScreen(this, Singleton.Instance.alisverisListele);
        }

        private void GecmisAlısverisListem_MouseHover(object sender, EventArgs e) => alisverislistem_label.Visible = true;

        private void GecmisAlısverisListem_MouseLeave(object sender, EventArgs e)=> alisverislistem_label.Visible = false;

        private void Siparis_listele_btn_MouseHover(object sender, EventArgs e) => Siparis_listele_label.Visible = true;
   
        private void Siparis_listele_btn_MouseLeave(object sender, EventArgs e) => Siparis_listele_label.Visible = false;


        private void Siparis_listele_btn_Click(object sender, EventArgs e)
        {
            Singleton.Instance.listeleSiparisler.SiparislerimListeleOnLoad();
            Singleton.Instance.ChangeScreen(this, Singleton.Instance.listeleSiparisler);
        }
       
    }
}
