using InvoicesDatabase.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvoicesDatabase;

public static class DatabaseSeeder {

    public static void Seed(InvoicesDbContext context) {
        if (context.Invoices.Any()) return;

        // ── Items ────────────────────────────────────────────────────────────
        var itemConsulting  = new Item { Id = Guid.Parse("11111111-0000-0000-0000-000000000001"), Name = "Consulting Services" };
        var itemLicense     = new Item { Id = Guid.Parse("11111111-0000-0000-0000-000000000002"), Name = "Software License" };
        var itemSupport     = new Item { Id = Guid.Parse("11111111-0000-0000-0000-000000000003"), Name = "Technical Support" };
        var itemTraining    = new Item { Id = Guid.Parse("11111111-0000-0000-0000-000000000004"), Name = "Training Package" };
        var itemHardware    = new Item { Id = Guid.Parse("11111111-0000-0000-0000-000000000005"), Name = "Hardware Equipment" };

        context.Items.AddRange(itemConsulting, itemLicense, itemSupport, itemTraining, itemHardware);

        // ── Contractors ──────────────────────────────────────────────────────
        var contractorAlpha = new Contractor {
            Id             = Guid.Parse("22222222-0000-0000-0000-000000000001"),
            ContractorId   = 1001,
            ContractorName = "Alpha Solutions sp. z o.o.",
            Address        = "ul. Marszałkowska 1",
            Country        = "PL",
            PostCode       = "00-001"
        };
        var contractorBeta = new Contractor {
            Id             = Guid.Parse("22222222-0000-0000-0000-000000000002"),
            ContractorId   = 1002,
            ContractorName = "Beta Systems GmbH",
            Address        = "Hauptstraße 42",
            Country        = "DE",
            PostCode       = "10115"
        };
        var contractorGamma = new Contractor {
            Id             = Guid.Parse("22222222-0000-0000-0000-000000000003"),
            ContractorId   = 1003,
            ContractorName = "Gamma Technologies Ltd.",
            Address        = "10 Downing Street",
            Country        = "GB",
            PostCode       = "SW1A 2AA"
        };

        context.Contractors.AddRange(contractorAlpha, contractorBeta, contractorGamma);

        // ── Helper to build an InvoicePosition ───────────────────────────────
        static InvoicePosition Pos(Guid invoiceId, Item item, double qty, double price, double discount,
                                   string account, string description) => new() {
            Id            = Guid.NewGuid(),
            InvoiceId     = invoiceId,
            ItemId        = item.Id,
            Quantity      = qty,
            UnitPrice     = price,
            Discount      = discount,
            AccountNumber = account,
            Description   = description
        };

        static double Net(params (double qty, double price, double discount)[] lines)
            => Math.Round(lines.Sum(l => l.qty * l.price * (1 - l.discount)), 2);

        // ── 2025 Invoices (6) ────────────────────────────────────────────────

        var inv2025_01Id = Guid.Parse("33333333-0000-0000-0000-000000000001");
        var inv2025_01 = new Invoice {
            Id            = inv2025_01Id,
            ContractorId  = contractorAlpha.Id,
            InvoiceNumber = "FV/2025/01/001",
            Description   = "January consulting and licensing",
            SaleDate      = new DateTime(2025, 1, 20),
            InvoiceDate   = new DateTime(2025, 1, 20),
            DueDate       = new DateTime(2025, 2, 3),
            VatRate       = 0.23,
            NetValue      = Net((10, 200, 0), (2, 1500, 0.05)),
            Currency      = "PLN",
            Paid          = true
        };

        var inv2025_02Id = Guid.Parse("33333333-0000-0000-0000-000000000002");
        var inv2025_02 = new Invoice {
            Id            = inv2025_02Id,
            ContractorId  = contractorBeta.Id,
            InvoiceNumber = "FV/2025/02/001",
            Description   = "Software support contract Q1",
            SaleDate      = new DateTime(2025, 2, 14),
            InvoiceDate   = new DateTime(2025, 2, 14),
            DueDate       = new DateTime(2025, 2, 28),
            VatRate       = 0.19,
            NetValue      = Net((1, 5000, 0), (8, 150, 0)),
            Currency      = "EUR",
            Paid          = true
        };

        var inv2025_03Id = Guid.Parse("33333333-0000-0000-0000-000000000003");
        var inv2025_03 = new Invoice {
            Id            = inv2025_03Id,
            ContractorId  = contractorGamma.Id,
            InvoiceNumber = "FV/2025/03/001",
            Description   = "Training package delivery",
            SaleDate      = new DateTime(2025, 3, 10),
            InvoiceDate   = new DateTime(2025, 3, 10),
            DueDate       = new DateTime(2025, 3, 24),
            VatRate       = 0.20,
            NetValue      = Net((3, 800, 0.1), (1, 200, 0)),
            Currency      = "GBP",
            Paid          = true
        };

        var inv2025_04Id = Guid.Parse("33333333-0000-0000-0000-000000000004");
        var inv2025_04 = new Invoice {
            Id            = inv2025_04Id,
            ContractorId  = contractorAlpha.Id,
            InvoiceNumber = "FV/2025/06/001",
            Description   = "Mid-year hardware procurement",
            SaleDate      = new DateTime(2025, 6, 15),
            InvoiceDate   = new DateTime(2025, 6, 15),
            DueDate       = new DateTime(2025, 6, 29),
            VatRate       = 0.23,
            NetValue      = Net((5, 2200, 0), (10, 50, 0.05)),
            Currency      = "PLN",
            Paid          = true
        };

        var inv2025_05Id = Guid.Parse("33333333-0000-0000-0000-000000000005");
        var inv2025_05 = new Invoice {
            Id            = inv2025_05Id,
            ContractorId  = contractorBeta.Id,
            InvoiceNumber = "FV/2025/09/001",
            Description   = "License renewal Q3",
            SaleDate      = new DateTime(2025, 9, 1),
            InvoiceDate   = new DateTime(2025, 9, 1),
            DueDate       = new DateTime(2025, 9, 15),
            VatRate       = 0.19,
            NetValue      = Net((3, 1800, 0), (5, 300, 0.1)),
            Currency      = "EUR",
            Paid          = false
        };

        var inv2025_06Id = Guid.Parse("33333333-0000-0000-0000-000000000006");
        var inv2025_06 = new Invoice {
            Id            = inv2025_06Id,
            ContractorId  = contractorGamma.Id,
            InvoiceNumber = "FV/2025/12/001",
            Description   = "Year-end consulting wrap-up",
            SaleDate      = new DateTime(2025, 12, 10),
            InvoiceDate   = new DateTime(2025, 12, 10),
            DueDate       = new DateTime(2025, 12, 24),
            VatRate       = 0.20,
            NetValue      = Net((15, 200, 0), (2, 500, 0)),
            Currency      = "GBP",
            Paid          = false
        };

        // ── 2026 Invoices (5) ────────────────────────────────────────────────

        var inv2026_01Id = Guid.Parse("33333333-0000-0000-0000-000000000007");
        var inv2026_01 = new Invoice {
            Id            = inv2026_01Id,
            ContractorId  = contractorAlpha.Id,
            InvoiceNumber = "FV/2026/01/001",
            Description   = "New year consulting kickoff",
            SaleDate      = new DateTime(2026, 1, 5),
            InvoiceDate   = new DateTime(2026, 1, 5),
            DueDate       = new DateTime(2026, 1, 19),
            VatRate       = 0.23,
            NetValue      = Net((20, 200, 0), (1, 3000, 0.05)),
            Currency      = "PLN",
            Paid          = true
        };

        var inv2026_02Id = Guid.Parse("33333333-0000-0000-0000-000000000008");
        var inv2026_02 = new Invoice {
            Id            = inv2026_02Id,
            ContractorId  = contractorBeta.Id,
            InvoiceNumber = "FV/2026/02/001",
            Description   = "Support contract renewal",
            SaleDate      = new DateTime(2026, 2, 1),
            InvoiceDate   = new DateTime(2026, 2, 1),
            DueDate       = new DateTime(2026, 2, 15),
            VatRate       = 0.19,
            NetValue      = Net((1, 6000, 0), (12, 150, 0)),
            Currency      = "EUR",
            Paid          = true
        };

        var inv2026_03Id = Guid.Parse("33333333-0000-0000-0000-000000000009");
        var inv2026_03 = new Invoice {
            Id            = inv2026_03Id,
            ContractorId  = contractorGamma.Id,
            InvoiceNumber = "FV/2026/03/001",
            Description   = "Advanced training programme",
            SaleDate      = new DateTime(2026, 3, 20),
            InvoiceDate   = new DateTime(2026, 3, 20),
            DueDate       = new DateTime(2026, 4, 3),
            VatRate       = 0.20,
            NetValue      = Net((4, 800, 0.1), (2, 200, 0)),
            Currency      = "GBP",
            Paid          = false
        };

        var inv2026_04Id = Guid.Parse("33333333-0000-0000-0000-000000000010");
        var inv2026_04 = new Invoice {
            Id            = inv2026_04Id,
            ContractorId  = contractorAlpha.Id,
            InvoiceNumber = "FV/2026/05/001",
            Description   = "Hardware upgrade project",
            SaleDate      = new DateTime(2026, 5, 12),
            InvoiceDate   = new DateTime(2026, 5, 12),
            DueDate       = new DateTime(2026, 5, 26),
            VatRate       = 0.23,
            NetValue      = Net((8, 2200, 0), (20, 50, 0.05)),
            Currency      = "PLN",
            Paid          = false
        };

        var inv2026_05Id = Guid.Parse("33333333-0000-0000-0000-000000000011");
        var inv2026_05 = new Invoice {
            Id            = inv2026_05Id,
            ContractorId  = contractorBeta.Id,
            InvoiceNumber = "FV/2026/07/001",
            Description   = "Q3 license expansion",
            SaleDate      = new DateTime(2026, 7, 1),
            InvoiceDate   = new DateTime(2026, 7, 1),
            DueDate       = new DateTime(2026, 7, 15),
            VatRate       = 0.19,
            NetValue      = Net((5, 1800, 0), (6, 300, 0.1)),
            Currency      = "EUR",
            Paid          = false
        };

        context.Invoices.AddRange(
            inv2025_01, inv2025_02, inv2025_03,
            inv2025_04, inv2025_05, inv2025_06,
            inv2026_01, inv2026_02, inv2026_03, inv2026_04, inv2026_05);

        // ── Positions ────────────────────────────────────────────────────────
        context.InvoicePositions.AddRange(
            // inv2025_01
            Pos(inv2025_01Id, itemConsulting, 10, 200,    0,    "401-01", "Consulting hours January"),
            Pos(inv2025_01Id, itemLicense,     2, 1500, 0.05,   "402-01", "Annual software license x2"),
            // inv2025_02
            Pos(inv2025_02Id, itemSupport,     1, 5000,    0,   "403-01", "Annual support contract"),
            Pos(inv2025_02Id, itemConsulting,  8,  150,    0,   "401-01", "Remote consulting days"),
            // inv2025_03
            Pos(inv2025_03Id, itemTraining,    3,  800, 0.10,   "404-01", "On-site training days"),
            Pos(inv2025_03Id, itemConsulting,  1,  200,    0,   "401-01", "Post-training advisory"),
            // inv2025_04
            Pos(inv2025_04Id, itemHardware,    5, 2200,    0,   "405-01", "Server rack units"),
            Pos(inv2025_04Id, itemSupport,    10,   50, 0.05,   "403-01", "Onsite installation per unit"),
            // inv2025_05
            Pos(inv2025_05Id, itemLicense,     3, 1800,    0,   "402-01", "License renewal 3 seats"),
            Pos(inv2025_05Id, itemSupport,     5,  300, 0.10,   "403-01", "Extended support hours"),
            // inv2025_06
            Pos(inv2025_06Id, itemConsulting, 15,  200,    0,   "401-01", "Year-end consulting sessions"),
            Pos(inv2025_06Id, itemTraining,    2,  500,    0,   "404-01", "Documentation training"),
            // inv2026_01
            Pos(inv2026_01Id, itemConsulting, 20,  200,    0,   "401-01", "Consulting hours January"),
            Pos(inv2026_01Id, itemLicense,     1, 3000, 0.05,   "402-01", "Enterprise license renewal"),
            // inv2026_02
            Pos(inv2026_02Id, itemSupport,     1, 6000,    0,   "403-01", "Annual support contract 2026"),
            Pos(inv2026_02Id, itemConsulting, 12,  150,    0,   "401-01", "Remote consulting days"),
            // inv2026_03
            Pos(inv2026_03Id, itemTraining,    4,  800, 0.10,   "404-01", "Advanced training days"),
            Pos(inv2026_03Id, itemConsulting,  2,  200,    0,   "401-01", "Post-training advisory"),
            // inv2026_04
            Pos(inv2026_04Id, itemHardware,    8, 2200,    0,   "405-01", "Workstation upgrades"),
            Pos(inv2026_04Id, itemSupport,    20,   50, 0.05,   "403-01", "Installation service per unit"),
            // inv2026_05
            Pos(inv2026_05Id, itemLicense,     5, 1800,    0,   "402-01", "License expansion 5 seats"),
            Pos(inv2026_05Id, itemSupport,     6,  300, 0.10,   "403-01", "Extended support hours Q3")
        );

        context.SaveChanges();
    }
}
