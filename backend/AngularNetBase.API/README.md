## Database - Migration creation scripts

```
Add-Migration -Name Init -Context IdentityContext -Project Identity -StartupProject AngularNetBase.API -OutputDir Infrastructure/Migrations
```

## Database - DB update scripts

```
Update-Database -Context IdentityContext -Project Identity -StartupProject AngularNetBase.API
```