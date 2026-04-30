# Deployment Assets

This folder contains the assets and automation used to deploy College Course Tracker to GitLab Pages for Task 4.

## Files

- `build-pages.ps1` generates the `public/` folder used by GitLab Pages.
- `export-project.ps1` creates a compressed archive of the current repository state.
- `Dockerfile` packages the generated static site into an Nginx container image.
- `nginx/default.conf` provides a simple route-safe Nginx configuration.
- `site/` holds the deployment page template and stylesheet.

## Deployment target

- Cloud provider: GitLab Pages
- Repository: `master` branch of `d424-software-engineering-capstone`
- Default deployment URL: `https://wgu-gitlab-environment.gitlab.io/student-repos/dcblakeman/d424-software-engineering-capstone/`

## Local generation

```powershell
pwsh -File .\deploy\build-pages.ps1 -OutputDir public
```

## Project export

```powershell
pwsh -File .\deploy\export-project.ps1
```
