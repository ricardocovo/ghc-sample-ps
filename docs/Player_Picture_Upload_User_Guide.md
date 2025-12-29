# Player Picture Upload User Guide

This guide explains how to upload, manage, and delete player profile pictures in the GhcSamplePs application.

## Overview

Player Picture Upload allows you to:
- Upload profile pictures for players from your device
- Display pictures on player profiles
- Delete and replace existing pictures
- Store pictures securely in Azure Blob Storage

## Key Features

| Feature | Description |
|---------|-------------|
| **File Upload** | Upload images directly from your device |
| **Supported Formats** | JPEG, PNG, GIF, WebP |
| **File Size Limit** | Maximum 5 MB per image |
| **Secure Storage** | Pictures stored in Azure Blob Storage with time-limited access |
| **Easy Management** | Delete and re-upload pictures anytime |

## Prerequisites

Before uploading player pictures, ensure:

1. You are signed in with appropriate permissions
2. The player exists in the system
3. You have owner permissions for the player (you created the player record)
4. Your image file meets the requirements:
   - Format: JPEG, PNG, GIF, or WebP
   - Size: 5 MB or less
   - Valid image content

## Uploading a Player Picture

### Step 1: Navigate to Edit Player Page

1. Go to the **Players** page
2. Find the player in the list (use search if needed)
3. Click the **Edit** button for that player
4. The **Player Information** tab will open by default

### Step 2: Upload the Picture

1. In the **Player Information** tab, locate the picture section at the top
2. Click the **Upload Picture** button (cloud upload icon)
3. A file browser will open
4. Select an image file from your device
5. The file will be validated automatically:
   - File size must not exceed 5 MB
   - Format must be JPEG, PNG, GIF, or WebP
   - Content type must match the file extension

### Step 3: Upload Progress

1. A progress indicator will appear during upload
2. The upload typically completes within 5 seconds for files up to 5 MB
3. Once complete, you'll see:
   - The uploaded picture displayed immediately
   - A success message confirming the upload
   - The picture is now associated with the player

### Validation Errors

If the upload fails, you'll see one of these error messages:

| Error Message | Cause | Solution |
|--------------|-------|----------|
| "The selected file exceeds the 5 MB size limit. Please choose a smaller image." | File is larger than 5 MB | Choose a smaller image or compress the existing one |
| "Invalid file format. Please upload a JPEG, PNG, GIF, or WebP image." | Unsupported file format | Convert the image to a supported format |
| "File must have an extension." | File name has no extension | Rename the file to include an extension (e.g., .jpg) |
| "The uploaded file appears to be empty." | File has no content | Select a valid image file |
| "Failed to upload picture. Please try again." | Network or server error | Check your connection and retry |
| "You do not have permission to modify this player's picture." | Authorization failure | Ensure you own the player record |

## Viewing Player Pictures

### On Edit Player Page

1. Navigate to the player's **Edit Player** page
2. The picture appears at the top of the **Player Information** tab
3. If no picture is uploaded, you'll see a placeholder avatar icon
4. Pictures are displayed in a circular or rounded square container

### Picture Display Performance

- Pictures load within 1 second on average
- Images are retrieved securely from Azure Blob Storage
- Time-limited access tokens ensure security
- Pictures are displayed at an optimized size for fast loading

## Replacing an Existing Picture

### Option 1: Upload Over Existing Picture

1. Navigate to the player's **Edit Player** page
2. Click the **Upload Picture** button
3. Select a new image file
4. The new picture will automatically replace the old one
5. No need to delete the old picture first

### Option 2: Delete Then Upload

1. Delete the existing picture (see below)
2. Follow the upload steps to add a new picture

**Note:** When replacing a picture, the old picture is automatically deleted from storage to save space.

## Deleting a Player Picture

### Step 1: Access Delete Option

1. Navigate to the player's **Edit Player** page
2. In the picture section, locate the **Delete** button (trash icon)
3. The delete button is only visible when a picture exists

### Step 2: Confirm Deletion

1. Click the **Delete** button
2. A confirmation dialog will appear asking "Are you sure you want to delete this player's picture?"
3. Click **Yes** to confirm deletion or **No** to cancel

### Step 3: Deletion Complete

1. The picture is removed from both Azure Storage and the player record
2. You'll see a success message confirming deletion
3. The placeholder avatar icon will appear
4. You can immediately upload a new picture if desired

**Note:** Picture deletion is permanent and cannot be undone. Make sure you want to delete the picture before confirming.

## Troubleshooting

### Upload Fails

**Problem:** Upload button doesn't respond or upload fails immediately

**Solutions:**
1. Check your internet connection
2. Verify the file is a valid image format
3. Try a smaller file size
4. Refresh the page and try again

---

**Problem:** "Upload failed due to network error"

**Solutions:**
1. Check your internet connection stability
2. Try again - the application may retry automatically
3. If the problem persists, contact your administrator

---

**Problem:** "Unable to upload picture due to storage limitations"

**Solutions:**
1. Contact your administrator - the Azure Storage account may be at capacity
2. Try again later

### Picture Doesn't Display

**Problem:** Picture uploaded successfully but doesn't appear

**Solutions:**
1. Refresh the page
2. Clear your browser cache
3. Check if you're still signed in - sign out and sign back in if needed
4. Contact support if the issue persists

---

**Problem:** Placeholder/avatar shows instead of picture

**Solutions:**
1. Verify the picture was uploaded successfully (check for success message)
2. Refresh the page
3. If the problem persists, try re-uploading the picture

### Authorization Issues

**Problem:** "You do not have permission to modify this player's picture"

**Solutions:**
1. Verify you are signed in
2. Check that you created the player record (only owners can manage pictures)
3. Contact your administrator if you need access

## Best Practices

### Image Quality

- **Resolution**: Use images at least 300x300 pixels for best quality
- **Aspect Ratio**: Square images (1:1) work best for circular displays
- **File Format**: JPEG is best for photographs, PNG for graphics with transparency
- **File Size**: Optimize images before upload - aim for under 1 MB for faster uploads

### Privacy & Consent

- Always obtain consent before uploading pictures of minors
- Ensure parents/guardians have approved the use of player images
- Follow your organization's privacy policies and GDPR requirements

### Maintenance

- Update pictures periodically to keep profiles current
- Remove pictures of players who leave the organization
- Keep image quality consistent across all player profiles

## Security & Privacy

### How Pictures Are Protected

| Security Feature | Description |
|-----------------|-------------|
| **Authentication Required** | Only signed-in users can upload/delete pictures |
| **Owner Authorization** | Only the user who created the player record can manage pictures |
| **Private Storage** | Pictures stored in private Azure Blob Storage (not publicly accessible) |
| **Time-Limited Access** | Access URLs expire after 1 hour for security |
| **Encrypted Transfer** | All uploads use HTTPS encryption |
| **Audit Logging** | All uploads and deletions are logged for compliance |

### Data Privacy Compliance

- Pictures are stored in Canada Central region for GDPR compliance
- Access is restricted to authorized users only
- Pictures are deleted when player records are deleted
- Audit trails are maintained for compliance reporting

## FAQ

**Q: Can I upload pictures for players I didn't create?**

A: No, only the user who created the player record can upload or delete pictures for that player. This ensures data ownership and privacy.

---

**Q: What happens to pictures when I delete a player?**

A: When a player is deleted, their picture is automatically removed from Azure Storage to prevent orphaned files and save storage space.

---

**Q: Can I upload multiple pictures for one player?**

A: Currently, each player can have one profile picture. If you upload a new picture, it replaces the existing one.

---

**Q: How long does an upload take?**

A: Most uploads complete within 5 seconds for files up to 5 MB. Upload time depends on your internet connection speed and file size.

---

**Q: Are pictures backed up?**

A: Yes, Azure Blob Storage provides automatic redundancy and backup. Pictures are stored with Local Redundant Storage (LRS) by default.

---

**Q: Can I download a player's picture?**

A: You can view pictures in the application, but downloading is not currently supported through the UI. Contact your administrator if you need to export player pictures.

---

**Q: What if I accidentally delete a picture?**

A: Picture deletion is permanent. You'll need to re-upload the picture. Make sure to confirm deletion carefully.

## Additional Resources

### Related Documentation

- [Player Management Guide](playerstats-requirements.md) - General player management
- [Team Management User Guide](Team_Management_User_Guide.md) - Managing team assignments
- [Player Statistics User Guide](Player_Statistics_User_Guide.md) - Recording game statistics
- [Azure Entra ID Setup Guide](Azure_EntraID_Setup_Guide.md) - Authentication configuration

### Support

If you encounter issues not covered in this guide:

1. Check the application logs (if you have admin access)
2. Contact your system administrator
3. Refer to the [Development Environment Setup](Development_Environment_Setup.md) for technical details

---

**Last Updated:** December 29, 2024  
**Version:** 1.0.0  
**Feature:** Player Picture Upload and Management
