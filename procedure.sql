delimiter $$
create definer=`root`@`localhost` procedure `prijava`(
    in korisnickoIme varchar(50), 
    in lozinka varchar(50), 
    out prijava boolean, 
    out idZaposleni varchar(13), 
    out uloga varchar(45), 
    out ime varchar(45), 
    out prezime varchar(45), 
    out brojTelefona varchar(20), 
    out email varchar(255), 
    out specijalizacija varchar(100)
)
begin
    declare user_count int default 0;
    
    select count(*) into user_count 
    from zaposleni z
    where z.KorisnickoIme = korisnickoIme and z.Lozinka = lozinka;

    set prijava = (user_count > 0);

    if prijava then
    select 
        z.JMBG, 
        z.Uloga, 
        z.Ime, 
        z.Prezime, 
        z.BrojTelefona, 
        z.Email, 
        z.Specijalizacija
    into 
        idZaposleni, 
        uloga, 
        ime, 
        prezime, 
        brojTelefona, 
        email, 
        specijalizacija
    from zaposleni z
    where z.KorisnickoIme = korisnickoIme 
      and z.Lozinka = lozinka
    limit 1;

    set idZaposleni = idZaposleni;
    set uloga = uloga;
    set ime = ime;
    set prezime = prezime;
    set brojTelefona = brojTelefona;
    set email = email;
    set specijalizacija = specijalizacija;
end if;
end $$
delimiter ;


delimiter $$
create trigger trg_kreiraj_racun_nakon_stavke
after insert on tretman_stavka
for each row
begin
  declare ukupno decimal(8,2);
  declare jmbgPacijenta varchar(13);
  declare datumRacuna date;
  declare jmbgTehnicara varchar(13);
  declare idTermina int;
  declare racun_id int;
  declare materijal_ukupno decimal(8,2) default 0;

  select r.idRacuna into racun_id
  from racun r
  where r.idPregleda = new.idPregleda
  limit 1;

  select p.idTermina, p.JMBGMedicinskogTehnicara
  into idTermina, jmbgTehnicara
  from pregled p
  where p.idPregleda = new.idPregleda;

  select t.jmbgPacijenta, date(t.datumIVrijeme)
  into jmbgPacijenta, datumRacuna
  from termin t
  where t.idTermina = idTermina;

  select ifnull(sum(ts.cijena), 0) into ukupno
  from tretman_stavka ts
  where ts.idPregleda = new.idPregleda;

  SELECT IFNULL(SUM(ms.Kolicina * m.CijenaPoJedinici), 0) into materijal_ukupno
  from materijal_stavka ms
  inner join materijal m on ms.idMaterijala = m.idMaterijala
  where ms.idPregleda = new.idPregleda;

  set ukupno = ukupno + materijal_ukupno;

  if racun_id is not null then
    update racun 
    set iznos = ukupno, 
        datumIzdavanja = now(),
        jmbgPacijenta = jmbgPacijenta,
        jmbgMedicinskogTehnicara = jmbgTehnicara
    where idRacuna = racun_id;
  else
    insert into racun (iznos, datumIzdavanja, jmbgPacijenta, jmbgMedicinskogTehnicara, idPregleda)
    values (ukupno, now(), jmbgPacijenta, jmbgTehnicara, new.idPregleda);
  end if;
end $$
delimiter ;


delimiter $$
create procedure dodaj_materijal_za_pregled(
    in p_idPregleda int,
    in p_idMaterijala int,
    in p_Kolicina double
)
begin
    declare trenutna_zaliha double;

    select TrenutnaZaliha into trenutna_zaliha
    from materijal where idMaterijala = p_idMaterijala;

    insert into materijal_stavka (idPregleda, idMaterijala, Kolicina)
    value (p_idPregleda, p_idMaterijala, least(p_Kolicina, trenutna_zaliha));

    update materijal
    set TrenutnaZaliha = TrenutnaZaliha - least(p_Kolicina, trenutna_zaliha)
    where idMaterijala = p_idMaterijala;
end$$
delimiter ;


drop trigger upozorenje_zalihe; 
delimiter $$
create trigger upozorenje_zalihe
after update on materijal
for each row
begin
    if new.TrenutnaZaliha < new.MinimalnaZaliha then
        insert into upozorenja(idMaterijala, poruka)
        values (new.idMaterijala, CONCAT('Zaliha materijala (ID=', NEW.idMaterijala,') je ispod minimuma!'));
    end if;
end$$
delimiter ;

