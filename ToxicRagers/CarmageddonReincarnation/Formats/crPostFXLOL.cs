using System;
using System.IO;

namespace ToxicRagers.CarmageddonReincarnation.Formats
{
    public class PostFX
    {
        public void Save(string path)
        {
            File.WriteAllText(path,
                @"
                    module((...), post_fx)
                    middle_gray = 1
                    dof_focus_distance = 20
                    dof_focus_sensitivity = 0.3
                    dof_focus_speedlimit = 20000
                    dof_aperture_number = 0.6
                    aa_falloff_start = 0
                    aa_falloff_end = 3000
                    blur_maxlength = 0.4
                    blur_sampleinterleaved = 1
                    blur_falloff_start = 100
                    blur_falloff_end = 8
                    blur_shutter_speed = 0.004
                    cam_rot_threshold = 0
                    cam_trans_threshold = 0.5
                    velmask_dot_threshold = 0
                    velmask_dist_threshold = 0
                    velmask_target_distance = 0
                    tonemap_exposure = 1
                    auto_exposure_delay = 0.1
                    tonemap_gamma = 0.5
                    tonemapping_factor = 0.4
                    adaptation_sensitivity = 0.5
                    adaptation_scale = 0.5
                    adaptation_speed_limit = 10000
                    light_dark_border = 1
                    metering_xy_width_height = {
                      0,
                      0,
                      1,
                      1
                    }
                    glare_luminance = 60
                    glare_threshold = 0.28
                    glare_remap_factor = 0.5
                    glare_bloom_gamma = 1
                    glare_result_blur = 1
                    glare_star_softness = -1
                    glare_gen_range_scale = 1
                    glare_star_fov_depend = -2
                    glare_guassian_scale = 4
                    glare_bloom_filter_thresh = 0.01
                    glare_star_filter_thresh = 0
                    lensflare_luminance = 0.5
                    lensflare_ghostscale = 0.5
                    colour_saturation = {
                      0.86,
                      0.9,
                      0.82
                    }
                    colour_contrast = {
                      0.94,
                      1.02,
                      1.04
                    }
                    colour_modulation = {
                      0.84,
                      0.96,
                      0.9
                    }
                    colour_hsb = {
                      0,
                      1.2,
                      1
                    }
                    colour_bias = {
                      0,
                      0,
                      0
                    }
                    white_bal_lum = {6400, 1.2}
                    lateral_dispersion = {0.004, 0.002}
                    uniform_dispersion = {0, 0}
                    blur_samples = -1
                    blur_max_recurences = -1
                    glare_num_bloom_levels = -1
                    chr_ab_num_samples = 4
                    tonemap_func = ""log""
                    colour_space = ""hdr""
                    glare_shape = ""lens_flare""
                    dof_screen_pos = {0.5, 0.5}
                    tonemap_exposure_range = {-1000, 1000}
                    lensflare_colour = {
                      13,
                      13,
                      26
                    }
                    lensflare_ghost_colour = {
                      51,
                      127,
                      255
                    }
                    hdr_enabled = true
                    aa_enabled = true
                    motionblur_enabled = true
                    heathaze_enabled = false
                    dof_enabled = true
                    enable_auto_exposure = true
                    enable_auto_focus = false
                    do_not_passover_focus = false
                    camera_only_moved = false
                    anamorphic_lensflare = false
                    glare_affects_midgray = false
                    enable_col_correction = true
                    enable_chr_aberration = false"
            );
        }
    }
}
