alter procedure uspfn_SvRegisterSpkXls
	 @Area     varchar(50),
	 @Dealer   varchar(20),
	 @Outlet   varchar(20),
     @DateFrom date,
     @DateTo   date

as

with x as (
select a.CompanyCode     [Kode Dealer]
     , b.DealerName      [Name Dealer                   ]
     , a.BranchCode      [Kode Outlet]
     , c.BranchName      [Nama Outlet                   ]
     , a.JobOrderNo      [No SPK     ]
     , a.JobOrderDate    [Tanggal SPK]
     , a.BasicModel      [Model      ]
     , a.PoliceRegNo     [No Polisi  ]
     , a.Odometer        [Odometer   ]
     , a.JobType         [Kode Pekerjaan]
     , a.JobTypeDesc     [Keterangan Pekerjaan]
     , a.TaskPartNo      [Kode Jasa / Part]
     , replace((case a.TaskPartType when 'T' then a.OperationName else a.PartName end), '', '') 
                         [Nama Jasa / Part              ]
     , a.DemandQty       [Demand Qty ]
     , a.SupplyQty       [Supply Qty ]
     , a.ReturnQty       [return Qty ]
     , a.SupplySlipNo    [No Supply Slip]
     , a.SSReturnNo      [No SS Return ]
     , a.SaName          [Nama SA      ]
     , a.FmName          [Nama Foreman ]
     , a.ServiceStatusDesc [Service Status]
  from SvRegisterSpk a
  left join DealerInfo b
    on b.DealerCode = a.CompanyCode
  left join OutletInfo c
    on c.CompanyCode = a.CompanyCode
   and c.BranchCode = a.BranchCode
  left join gnMstDealerMapping d
    on d.DealerCode = a.CompanyCode
 where 1 = 1
   and d.Area = (case @Area when '' then d.Area else @Area end)
   and a.CompanyCode = (case @Dealer when '' then a.CompanyCode else @Dealer end)
   and a.BranchCode = (case @Outlet when '' then a.BranchCode else @Outlet end)
   and a.JobOrderDate >= @DateFrom
   and convert(date, a.JobOrderDate) <= @DateTo
)
select * from x
--select [Node Err], count(x.JobOrderNo) from x group by [Node Err]


go

exec uspfn_SvRegisterSpkXls '','','','2014-09-01', '2014-09-04'
